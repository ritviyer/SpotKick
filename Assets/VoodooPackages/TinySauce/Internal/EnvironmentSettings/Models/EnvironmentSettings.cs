using System;
//using Voodoo.Sauce.Internal.RemoteConfig;

namespace Voodoo.Sauce.Internal.EnvironmentSettings
{
    public static class EnvironmentSettings
    {
        public enum Server
        {
            Tech = 0,
            Staging = 1,
            Dev = 2
        }

        public enum Api
        {
            RemoteConfig = 0,
            Analytics = 1
        }

        internal static Server GetServer(Api api)
        {
            switch (api)
            {
              
                case Api.Analytics:
                    return EnvironmentSettingsLocalStorage.GetAnalyticsServer();
                default:
                    return Server.Tech;
            }
        } 

        /// <summary>
        ///    <param>Return the full formatted url.</param>
        ///     <param name="path">Url with {0} instead of the server name</param>
        ///     <param name="api">Api is rely on the path</param>
        ///    <example>https://vs-api.voodoo-{0}.io/push-analytics</example>
        /// </summary>
        internal static string GetURL(string path, Api api)
        {
            //switch (api)
            //{

            //    case Api.Analytics:
            //        return String.Format(path, EnvironmentSettingsLocalStorage.GetAnalyticsServer());
            //    default:
            //        return String.Format(path, Server.Tech);
            //}

            return String.Format(path, Server.Tech);
        }
        
        internal static void SaveRemoteConfigServer(Server server,Api api)
        {
            switch (api)
            {
                case Api.RemoteConfig:
                    EnvironmentSettingsLocalStorage.SaveRemoteConfigServer(server);
                    break;
                case Api.Analytics:
                    EnvironmentSettingsLocalStorage.SaveAnalyticsServer(server);
                    break;
            }
        }

        //internal static RemoteConfigClusterApi.ConfigVersion GetRemoteConfigVersion() =>
        //    EnvironmentSettingsLocalStorage.GetRemoteConfigVersion();

        //internal static void SaveRemoteConfigVersion(RemoteConfigClusterApi.ConfigVersion configVersion) =>
        //    EnvironmentSettingsLocalStorage.SaveRemoteConfigVersion(configVersion);
    }
}