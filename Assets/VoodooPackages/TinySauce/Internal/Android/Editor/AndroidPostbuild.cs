using System.IO;
using UnityEditor.Android;

namespace Voodoo.Sauce.Internal.Editor
{
    public class AndroidPostBuild : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string projectPath)
        {
#if UNITY_2019_3_OR_NEWER
            projectPath += "/../";
#endif
            var fileInfo = new FileInfo(Path.Combine(projectPath, "gradle.properties"));
            string[] content = { "android.enableR8 = false","android.useAndroidX=true","android.enableJetifier = true"};
            string[] contentNew = { "android.enableR8 = false", "android.useAndroidX=true", "android.enableJetifier = true", "unityStreamingAssets=.unity3d**STREAMING_ASSETS**" };
#if UNITY_2020_1_OR_NEWER
            File.WriteAllLines(fileInfo.FullName, contentNew);
 

#else
            File.WriteAllLines(fileInfo.FullName, content);
 
#endif
            
        }
    }
}