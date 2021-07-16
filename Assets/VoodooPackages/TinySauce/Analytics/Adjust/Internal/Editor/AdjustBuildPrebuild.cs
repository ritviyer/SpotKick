using System.Collections.Generic;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Voodoo.Sauce.Internal.Editor;

namespace Voodoo.Sauce.Internal.Analytics.Editor
{
    public class AdjustBuildPrebuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateAdjustSettings(TinySauceSettings.Load());
        }

        public static void CheckAndUpdateAdjustSettings(TinySauceSettings sauceSettings)
        {
#if UNITY_IOS

            if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.adjustIOSToken.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAdjustToken);
#endif
#if UNITY_ANDROID
            if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.adjustAndroidToken.Replace(" ", string.Empty)))
                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAdjustToken);
#endif

        }
    }
}