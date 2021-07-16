using System;
using UnityEngine;

namespace Voodoo.Sauce.Internal.Analytics
{
    internal static class AnalyticsStorageHelper
    {
        private const string GameCountPrefKey = "VoodooSauce_GameCount";
        private const string SuccessfulGameCountPrefKey = "VoodooSauce_SuccessfulGameCount";
        private const string LevelPrefKey = "VoodooSauce_Level";
        private const string LaunchCountPrefKey = "VoodooSauce_AppLaunchCount";
        private const string HighestScorePrefKey = "VoodooSauce_HighScore";
        private const string FSShownCountPrefKey = "VoodooSauce_FSShownCount";
        private const string RVShownCountPrefKey = "VoodooSauce_RVShownCount";

        internal static int GetGameCount() => PlayerPrefs.GetInt(GameCountPrefKey, 0);

        internal static void IncrementGameCount()
        {
            int gameCount = GetGameCount() + 1;
            PlayerPrefs.SetInt(GameCountPrefKey, gameCount);
        }

        internal static int GetSuccessfulGameCount()
        {
            return PlayerPrefs.GetInt(SuccessfulGameCountPrefKey, 0);
        }

        internal static void IncrementSuccessfulGameCount()
        {
            int gameCount = GetSuccessfulGameCount() + 1;
            PlayerPrefs.SetInt(SuccessfulGameCountPrefKey, gameCount);
        }

        internal static double GetWinRate()
        {
            var gameCount = (double)GetGameCount();
            var successfulGameCount = (double)GetSuccessfulGameCount();

            return Math.Min(gameCount > 0 ? successfulGameCount / gameCount : 0, 1);
        }

        internal static bool HasWinRate()
        {
            return PlayerPrefs.HasKey(GameCountPrefKey) && PlayerPrefs.HasKey(SuccessfulGameCountPrefKey);
        }

        internal static string GetLevel()
        {
            return PlayerPrefs.GetString(LevelPrefKey);
        }

        internal static void UpdateLevel(string level)
        {
            if (!string.IsNullOrEmpty(level))
            {
                PlayerPrefs.SetString(LevelPrefKey, level);
            }
            else if (PlayerPrefs.HasKey(LevelPrefKey))
            {
                PlayerPrefs.DeleteKey(LevelPrefKey);
            }
        }

        internal static bool HasLevel() => PlayerPrefs.HasKey(LevelPrefKey);

        public static float GetGameHighestScore() => PlayerPrefs.GetFloat(HighestScorePrefKey, 0);

        internal static bool HasGameHighestScore()
        {
            return PlayerPrefs.HasKey(HighestScorePrefKey);
        }

        internal static bool UpdateGameHighestScore(float score)
        {
            if (!(score > GetGameHighestScore())) return false;
            PlayerPrefs.SetFloat(HighestScorePrefKey, score);
            return true;
        }

        internal static int GetAppLaunchCount() => PlayerPrefs.GetInt(LaunchCountPrefKey, 0);

        internal static bool IsFirstAppLaunch() => GetAppLaunchCount() == 1;

        internal static void IncrementAppLaunchCount()
        {
            int appLaunchCount = GetAppLaunchCount() + 1;
            PlayerPrefs.SetInt(LaunchCountPrefKey, appLaunchCount);
        }

        internal static int GetInterstitialShownCount() => PlayerPrefs.GetInt(FSShownCountPrefKey, 0);

        internal static int IncrementInterstitialShownCount()
        {
            int adCount = GetInterstitialShownCount() + 1;
            PlayerPrefs.SetInt(FSShownCountPrefKey, adCount);
            return adCount;
        }

        internal static int GetRewardedVideoShownCount() => PlayerPrefs.GetInt(RVShownCountPrefKey, 0);

        internal static int IncrementRewardedVideoShownCount()
        {
            int adCount = GetRewardedVideoShownCount() + 1;
            PlayerPrefs.SetInt(RVShownCountPrefKey, adCount);
            return adCount;
        }
    }
}