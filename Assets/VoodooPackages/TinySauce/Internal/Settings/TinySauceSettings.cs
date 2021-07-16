using UnityEngine;
using UnityEngine.Serialization;
using System;

using Voodoo.Sauce.Internal;

namespace Voodoo.Sauce.Internal
{
    [CreateAssetMenu(fileName = "Assets/Resources/TinySauce/TinySauceSettings", menuName = "TinySauce/Settings file")]
    public class TinySauceSettings : ScriptableObject
    {
        private const string SETTING_RESOURCES_PATH = "TinySauce/Settings";

        public static TinySauceSettings Load() => Resources.Load<TinySauceSettings>(SETTING_RESOURCES_PATH);


        [Header("Tiny Sauce version " + TinySauce.Version, order = 0)]
        [Header("GameAnalytics", order = 1)]
        [Tooltip("Your GameAnalytics Ios Game Key")]
        public string gameAnalyticsIosGameKey;

        [Tooltip("Your GameAnalytics Ios Secret Key")]
        public string gameAnalyticsIosSecretKey;

        [Tooltip("Your GameAnalytics Android Game Key")]
        public string gameAnalyticsAndroidGameKey;

        [Tooltip("Your GameAnalytics Android Secret Key")]
        public string gameAnalyticsAndroidSecretKey;
        [Header("Facebook")]
        [Tooltip("The Facebook App Id of your game")]
        public string facebookAppId;

        [Header("Adjust")]
        [Tooltip("The IOS Adjust App token of your game")]
        public string adjustIOSToken;
        [Tooltip("The Android Adjust App token of your game")]
        public string adjustAndroidToken;

        [Header("Voodoo Analytics Id (Editor Only)")]
        [ReadOnly]
        public string EditorIdfa;

        [Header("AB Test")]
        [HideInInspector]
        [Tooltip("Use Default ABTest or Voodoo Remote Config")]
        public bool UseRemoteConfig = false;

        [Header("Voodoo Analytics")]
        [ReadOnly]
        [Tooltip("Use Voodoo Analytics")]
        public bool UseVoodooAnalytics = true;

    }
}