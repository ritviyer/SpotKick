using System;
using UnityEngine;

namespace Voodoo.Sauce.Internal.Analytics
{
    public static class AnalyticsUserIdHelper
    {
        private const string PlayerPrefUserIdentifierKey = "VOODOO_ANALYTICS_USER_IDENTIFIER";

        public static string GetUserId()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefUserIdentifierKey))
            {
                PlayerPrefs.SetString(PlayerPrefUserIdentifierKey, Guid.NewGuid().ToString());
            }
            return PlayerPrefs.GetString(PlayerPrefUserIdentifierKey);
        }
    }
}