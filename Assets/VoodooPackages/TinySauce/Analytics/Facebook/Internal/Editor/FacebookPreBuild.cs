using System.Collections.Generic;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Voodoo.Sauce.Internal.Editor;

namespace Voodoo.Sauce.Internal.Analytics.Editor
{
    public class FacebookPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateFacebookSettings(TinySauceSettings.Load());
        }

        public static void CheckAndUpdateFacebookSettings(TinySauceSettings sauceSettings)
        {
#if UNITY_ANDROID || UNITY_IOS

      if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.facebookAppId))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.SettingsNoFacebookAppID);
            else
            {
                FacebookSettings.AppIds = new List<string> {sauceSettings.facebookAppId};
                FacebookSettings.AppLabels = new List<string> {Application.productName};
                EditorUtility.SetDirty(FacebookSettings.Instance);
            }      
#endif
            
        }
    }
}