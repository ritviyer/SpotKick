using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Facebook.Unity.Editor;
using System;

namespace Voodoo.Sauce.Internal.Editor
{
    public class AndroidPrebuild : IPreprocessBuildWithReport
    {
        private const string SourceFolderPath = "VoodooPackages/TinySauce/Internal/Android/Editor";
        private static readonly string SourceManifestPath = $"{SourceFolderPath}/AndroidManifest.xml";
        private static readonly string SourceGradlePath = $"{SourceFolderPath}/mainTemplate.gradle";
        private static readonly string SourceLauncherManifestPath = $"{SourceFolderPath}/LauncherManifest.xml";
        private static readonly string SourceLauncherGradlePath = $"{SourceFolderPath}/launcherTemplate.gradle";

        private const string PluginFolderPath = "Plugins";
        private const string AndroidFolderPath = "Plugins/Android";

        private static readonly string DestManifestPath = $"{AndroidFolderPath}/AndroidManifest.xml";
        private static readonly string DestGradlePath = $"{AndroidFolderPath}/mainTemplate.gradle";
        private static readonly string DestLauncherManifestPath = $"{AndroidFolderPath}/LauncherManifest.xml";
        private static readonly string DestLauncherGradlePath = $"{AndroidFolderPath}/launcherTemplate.gradle";

        private static readonly string AndroidGradleVersion = "3.4.3";
        private static readonly string AndroidBuildToolsGradleClasspath = $"classpath 'com.android.tools.build:gradle:{AndroidGradleVersion}'";

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android) {
                return;
            }

            CreateAndroidFolder();
            UpdateManifest();
            UpdateLauncherManifest();
            UpdateGradle();
            UpdateLauncherGradle();
            PreparePlayerSettings();
            PrepareResolver();
        }

        private static void PreparePlayerSettings()
        {
            // Set Android ARM64/ARMv7 Architecture   
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            // Set Android min version
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel19) {
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
            }
        }

        private static void PrepareResolver()
        {
            // Force playServices Resolver
            GooglePlayServices.PlayServicesResolver.Resolve(null, true);
        }

        private static void CreateAndroidFolder()
        {
            string pluginPath = Path.Combine(Application.dataPath, PluginFolderPath);
            string androidPath = Path.Combine(Application.dataPath, AndroidFolderPath);
            if (!Directory.Exists(pluginPath))
                Directory.CreateDirectory(pluginPath);
            if (!Directory.Exists(androidPath))
                Directory.CreateDirectory(androidPath);
        }

        private static void UpdateManifest()
        {
            string sourcePath = Path.Combine(Application.dataPath, SourceManifestPath);
            string content = File.ReadAllText(sourcePath)
#if UNITY_2019_3_OR_NEWER
                                 .Replace("attribute='**APPLICATION_ATTRIBUTES**'", string.Empty)
                                 .Replace("**APPLICATION_ATTRIBUTES_REPLACE**", string.Empty);
#else
                                 .Replace("attribute='**APPLICATION_ATTRIBUTES**'", $"android:icon=\"@mipmap/app_icon\"{Environment.NewLine}android:label=\"@string/app_name\"")
                                 .Replace("**APPLICATION_ATTRIBUTES_REPLACE**", ",icon,label");
#endif
            string destPath = Path.Combine(Application.dataPath, DestManifestPath);
            File.Delete(destPath);
            File.WriteAllText(destPath, content);
            //Add Facebook Manifest to  application manifest
            ManifestMod.GenerateManifest();
        }

        private static void UpdateLauncherManifest()
        {
#if UNITY_2019_3_OR_NEWER
            string sourcePath = Path.Combine(Application.dataPath, SourceLauncherManifestPath);
            string destPath = Path.Combine(Application.dataPath, DestLauncherManifestPath);
            File.Copy(sourcePath, destPath, true);
#endif
        }

        private static void UpdateLauncherGradle()
        {
#if UNITY_2019_3_OR_NEWER
            string sourcePath = Path.Combine(Application.dataPath, SourceLauncherGradlePath);
            string contentLauncher = File.ReadAllText(sourcePath)
                .Replace("**BUILD_SCRIPT_DEPS**", AndroidBuildToolsGradleClasspath);
#if UNITY_2020_1_OR_NEWER
        contentLauncher = contentLauncher.Replace("**STREAMING_ASSETS**]", "]+ unityStreamingAssets.tokenize(', ')");
#endif
            string destPath = Path.Combine(Application.dataPath, DestLauncherGradlePath);
            File.Delete(destPath);
            File.WriteAllText(destPath, contentLauncher);
#endif
        }


        private static void UpdateGradle()
        {
            string sourcePath = Path.Combine(Application.dataPath, SourceGradlePath);
            string content = File.ReadAllText(sourcePath)
#if UNITY_2019_3_OR_NEWER
                                 .Replace("**BUILD_SCRIPT_DEPS**", AndroidBuildToolsGradleClasspath)
                                 .Replace("**APPLY_PLUGINS**", "apply plugin: 'com.android.library'")
                                 .Replace("**APPLICATIONID**", string.Empty);
#if UNITY_2020_1_OR_NEWER
        content = content.Replace("**STREAMING_ASSETS**", " + unityStreamingAssets.tokenize(', ')");
                        
#endif
#else
                                 .Replace("**BUILD_SCRIPT_DEPS**", AndroidBuildToolsGradleClasspath)
                                 .Replace("**APPLY_PLUGINS**", "apply plugin: 'com.android.application'")
                                 .Replace("**APPLICATIONID**", "applicationId '**APPLICATIONID**'");
#endif

            string destPath = Path.Combine(Application.dataPath, DestGradlePath);
            File.Delete(destPath);
            File.WriteAllText(destPath, content);
        }
    }
}