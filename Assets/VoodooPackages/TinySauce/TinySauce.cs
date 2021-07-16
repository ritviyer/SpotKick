using GameAnalyticsSDK;
using Voodoo.Sauce.Internal;
using Voodoo.Sauce.Internal.Analytics;
using System.Collections.Generic;
//using UnityEditor.Build;
//using UnityEditor.Build.Reporting;


public static class TinySauce
{ 

    public const string Version = "5.0.3";

    public static string token = "";
    /// <summary>
    ///  Method to call whenever the user starts a game.
    /// </summary>
    /// <param name="levelNumber">The game Level, this parameter is optional for game without level</param>
    public static void OnGameStarted(string  levelNumber = null)
    {
        AnalyticsManager.TrackGameStarted(levelNumber);
    }
    public static void OnGameStarted(string levelNumber, Dictionary<string, object> eventProperties = null)
    {
        
        AnalyticsManager.TrackGameStarted(levelNumber, eventProperties);
    }
    
    /// <summary>
    /// Method to call whenever the user completes a game with levels.
    /// </summary>
    /// <param name="levelNumber">The game Level</param>
    /// <param name="score">The score of the game</param>


    public static void OnGameFinished(float score)
    {
        OnGameFinished(true, score);
    }

    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    /// <param name="eventProperties">An optional list of properties to send along with the event</param>
    public static void OnGameFinished(bool levelComplete, float score, Dictionary<string, object> eventProperties = null)
    {
        OnGameFinished(levelComplete, score, null, eventProperties);
    }

    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    /// <param name="levelNumber">The level number of the game</param>
    /// <param name="eventProperties">An optional list of properties to send along with the event</param>
    public static void OnGameFinished(bool levelComplete,
                                      float score,
                                      string levelNumber,
                                      Dictionary<string, object> eventProperties = null)
    {
        AnalyticsManager.TrackGameFinished(levelComplete, score, levelNumber, eventProperties);
        
    }



    /// <summary>
    /// Call this method to track any custom event you want.
    /// </summary>
    /// <param name="eventName">The name of the event to track</param>
    /// <param name="eventProperties">An optional list of properties to send along with the event</param>
    /// <param name="type">type of the event</param>
    /// <param name="analyticsProviders">The list of analytics provider you want to track your custom event to. If this list is null or empty, the event will be tracked in GameAnalytics and Mixpanel (if the user is in a cohort)</param>
    public static void TrackCustomEvent(string eventName,
                                        Dictionary<string, object> eventProperties = null,
                                        string type = null,
                                        List<AnalyticsProvider> analyticsProviders = null)
    {
        AnalyticsManager.TrackCustomEvent(eventName, eventProperties, type, analyticsProviders);
    }

    public static string UpdateAdjustToken(TinySauceSettings settings)
    {
#if UNITY_IOS
        token = settings.adjustIOSToken;
#endif
#if UNITY_ANDROID
        token = settings.adjustAndroidToken;
#endif

        return token;
    }


    public static string getToken()
    {
        TinySauceSettings sauceSettings = TinySauceSettings.Load();
#if UNITY_ANDROID
        return sauceSettings.adjustAndroidToken.Replace(" ", string.Empty);
#endif
#if UNITY_IOS
        return sauceSettings.adjustIOSToken.Replace(" ", string.Empty);
#endif
        return "";

    }
    public enum AnalyticsProvider
    {
        VoodooAnalytics,
        GameAnalytics,
    }

}
