using System;
using System.Collections.Generic;
using Facebook.Unity;
//using Voodoo.Sauce.Internal.Analytics;
using UnityEngine;


namespace Voodoo.Sauce.Internal.Analytics
{
    internal static class AnalyticsManager
    {
        private const string TAG = "VoodooAnalytics";
        private const string NO_GAME_LEVEL = "game";

        internal static bool HasGameStarted { get; private set; }


        // Voodoo sauce additional events
        internal static event Action<int, bool> OnGamePlayed;
        internal static event Action<string, Dictionary<string, object>> OnGameStartedEvent;
        internal static event Action<bool, float, string, Dictionary<string, object>> OnGameFinishedEvent;
        
        internal static event Action<string, float> OnTrackCustomValueEvent;
        internal static event Action OnApplicationFirstLaunchEvent;
        internal static event Action OnApplicationLaunchEvent;
        internal static event Action OnApplicationResumeEvent;
        
        internal static event Action<string, Dictionary<string, object>, string, List<TinySauce.AnalyticsProvider>> OnTrackCustomEvent;

        private static readonly List<TinySauce.AnalyticsProvider> DefaultAnalyticsProvider = new List<TinySauce.AnalyticsProvider>
            {TinySauce.AnalyticsProvider.GameAnalytics};

        private static readonly List<IAnalyticsProvider> AnalyticsProviders = new List<IAnalyticsProvider>()
        {
            new GameAnalyticsProvider(), new VoodooAnalyticsProvider(new VoodooAnalyticsParameters(false, true, "f7ec7203-4546-472b-b5c7-7df8c7a1fc40"))
        };

        internal static void Initialize(TinySauceSettings sauceSettings, bool consent)
        {
            // Initialize providers
            AnalyticsProviders.ForEach(provider => provider.Initialize(consent));
        }
        internal static void OnApplicationResume()
        {
            OnApplicationResumeEvent?.Invoke();
        }



        // Track game events
        // ================================================
        internal static void TrackApplicationLaunch()
        {
            AnalyticsStorageHelper.IncrementAppLaunchCount();
            //fire app launch events
            if (AnalyticsStorageHelper.IsFirstAppLaunch())
            {
                OnApplicationFirstLaunchEvent?.Invoke();
            }

            OnApplicationLaunchEvent?.Invoke();
        }


        internal static void TrackGameStarted(string level, Dictionary<string, object> eventProperties = null)
        {
            HasGameStarted = true;
            AnalyticsStorageHelper.IncrementGameCount();
            OnGameStartedEvent?.Invoke(level ?? NO_GAME_LEVEL, eventProperties);
        }

        internal static void TrackGameFinished(bool levelComplete, float score, string level, Dictionary<string, object> eventProperties)
        {
            HasGameStarted = false;
            AnalyticsStorageHelper.UpdateLevel(level);
            if (levelComplete)
            {
                // used to calculate the win rate (for RemoteConfig)
                AnalyticsStorageHelper.IncrementSuccessfulGameCount();
            }

            OnGamePlayed?.Invoke(AnalyticsStorageHelper.GetGameCount(), AnalyticsStorageHelper.UpdateGameHighestScore(score));
            OnGameFinishedEvent?.Invoke(levelComplete, score, level ?? NO_GAME_LEVEL, eventProperties);
        }

        internal static void TrackCustomEvent(string eventName,
                                              Dictionary<string, object> eventProperties,
                                              string type = null,
                                              List<TinySauce.AnalyticsProvider> analyticsProviders = null)
        {
            if (analyticsProviders == null || analyticsProviders.Count == 0)
            {
                analyticsProviders = DefaultAnalyticsProvider;
            }

            OnTrackCustomEvent?.Invoke(eventName, eventProperties, type, analyticsProviders);
        }
    }
}