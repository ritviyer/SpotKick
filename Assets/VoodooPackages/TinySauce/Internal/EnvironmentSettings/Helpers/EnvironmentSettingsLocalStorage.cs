using UnityEngine;
//using Voodoo.Sauce.Internal.RemoteConfig;

namespace Voodoo.Sauce.Internal.EnvironmentSettings
{
    public static class EnvironmentSettingsLocalStorage
    {
        private const string RemoteConfigServerKey = "RemoteConfigServer";
        private const string AnalyticsServerKey = "AnalyticsServer";
        private const string RemoteConfigVersionKey = "RemoteConfigVersion";
        
        //internal static EnvironmentSettings.Server GetRemoteConfigServer() => 
        //    (EnvironmentSettings.Server)PlayerPrefs.GetInt(RemoteConfigServerKey,0);
        
        internal static EnvironmentSettings.Server GetAnalyticsServer() => 
            (EnvironmentSettings.Server)PlayerPrefs.GetInt(AnalyticsServerKey,0);
        
        internal static void SaveRemoteConfigServer(EnvironmentSettings.Server server) => PlayerPrefs.SetInt(RemoteConfigServerKey,(int)server);
        
        internal static void SaveAnalyticsServer(EnvironmentSettings.Server server) => PlayerPrefs.SetInt(AnalyticsServerKey,(int)server);

        //internal static RemoteConfigClusterApi.ConfigVersion GetRemoteConfigVersion() =>
        //    (RemoteConfigClusterApi.ConfigVersion)PlayerPrefs.GetInt(RemoteConfigVersionKey, 0);


        //internal static void SaveRemoteConfigVersion(RemoteConfigClusterApi.ConfigVersion configVersion) =>
        //    PlayerPrefs.SetInt(RemoteConfigVersionKey, (int)configVersion);
    }
}