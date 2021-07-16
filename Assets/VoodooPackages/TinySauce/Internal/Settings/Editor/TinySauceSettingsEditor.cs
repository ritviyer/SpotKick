using System;
using UnityEditor;
using UnityEngine;
using Voodoo.Sauce.Internal.Analytics.Editor;

namespace Voodoo.Sauce.Internal.Editor
{
    [CustomEditor(typeof(TinySauceSettings))]
    public class TinySauceSettingsEditor : UnityEditor.Editor
    {
        private const string EditorPrefEditorIDFA = "EditorIDFA";
        private TinySauceSettings SauceSettings => target as TinySauceSettings;

        [MenuItem("TinySauce/TinySauce Settings/Edit Settings")]
        private static void EditSettings()
        {
            Selection.activeObject = CreateTinySauceSettings();
        }

        private static TinySauceSettings CreateTinySauceSettings()
        {
            TinySauceSettings settings = TinySauceSettings.Load();
            if (settings == null) {
                settings = CreateInstance<TinySauceSettings>();
                //create tinySauce folders if it not exists
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                if (!AssetDatabase.IsValidFolder("Assets/Resources/TinySauceSettings"))
                    AssetDatabase.CreateFolder("Assets/Resources", "TinySauce");
                //create TinySauceSettings file
                AssetDatabase.CreateAsset(settings, "Assets/Resources/TinySauce/Settings.asset");
                settings = TinySauceSettings.Load();
            }

            return settings;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(15);

#if UNITY_IOS || UNITY_ANDROID      
            if (GUILayout.Button(Environment.NewLine + "Check and Sync Settings" + Environment.NewLine)) {
                CheckAndUpdateSdkSettings(SauceSettings);
            }
#else
            EditorGUILayout.HelpBox(BuildErrorConfig.ErrorMessageDict[BuildErrorConfig.ErrorID.INVALID_PLATFORM], MessageType.Error);   
#endif

            string editorIdfa = EditorPrefs.GetString(EditorPrefEditorIDFA);
            if (string.IsNullOrEmpty(editorIdfa))
            {
                editorIdfa = Guid.NewGuid().ToString();
                EditorPrefs.SetString(EditorPrefEditorIDFA, editorIdfa);
            }

            SauceSettings.EditorIdfa = editorIdfa;
        }

        private static void CheckAndUpdateSdkSettings(TinySauceSettings sauceSettings)
        {
            Console.Clear();
            BuildErrorWindow.Clear();
            GameAnalyticsPreBuild.CheckAndUpdateGameAnalyticsSettings(sauceSettings);
            FacebookPreBuild.CheckAndUpdateFacebookSettings(sauceSettings);
            AdjustBuildPrebuild.CheckAndUpdateAdjustSettings(sauceSettings);       
            TinySauce.UpdateAdjustToken(sauceSettings);
           
            

        }
    }
}