using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

namespace Voodoo.Sauce.Internal.Analytics
{
    public static class GameAnalyticsWrapper
    {
        private const string TAG = "GameAnalyticsWrapper";

        private static bool _isInitialized;
        private static bool _isDisabled;

        private static readonly Queue<GameAnalyticsEvent> QueuedEvents = new Queue<GameAnalyticsEvent>();

        internal static bool Initialize(bool consent)
        {
            if (!consent) {
                Disable();
                return _isInitialized;
            }

            InstantiateGameAnalytics();
            VoodooLog.Log(TAG, "GameAnalytics initialized, tracking pending events: " + QueuedEvents.Count);
            while (QueuedEvents.Count > 0) {
                QueuedEvents.Dequeue().Track();
            }

            _isInitialized = true;
            return _isInitialized;
        }

        internal static void TrackProgressEvent(GAProgressionStatus status, string progress, int? score)
        {
            if (_isDisabled) return;

            var progressEvent = new ProgressEvent {
                status = status,
                progress = progress,
                score = score
            };
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + status);
                QueuedEvents.Enqueue(progressEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event " + status + " to GameAnalytics");
                progressEvent.Track();
            }
        }

        internal static void TrackDesignEvent(string eventName, float? eventValue)
        {
            if (_isDisabled) return;

            var designEvent = new DesignEvent {
                eventName = eventName,
                eventValue = eventValue
            };
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + eventName);
                QueuedEvents.Enqueue(designEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event " + eventName + " to GameAnalytics");
                designEvent.Track();
            }
        }
        

        private static void InstantiateGameAnalytics()
        {
            var gameAnalyticsComponent = Object.FindObjectOfType<GameAnalytics>();
            if (gameAnalyticsComponent == null) {
                var gameAnalyticsGameObject = new GameObject("GameAnalytics");
                gameAnalyticsGameObject.AddComponent<GameAnalytics>();
                gameAnalyticsGameObject.SetActive(true);
            } else {
                gameAnalyticsComponent.gameObject.name = "GameAnalytics";
            }

            GameAnalytics.Initialize();
        }

        private static void Disable()
        {
            VoodooLog.Log(TAG, "Disabling GameAnalytics No User Consent.");
            _isDisabled = true;
            QueuedEvents.Clear();
        }

        private abstract class GameAnalyticsEvent
        {
            public abstract void Track();
        }

        private class ProgressEvent : GameAnalyticsEvent
        {
            public GAProgressionStatus status;
            public string progress;
            public int? score;

            public override void Track()
            {
                if (score != null) {
                    GameAnalytics.NewProgressionEvent(status, progress, (int) score);
                } else {
                    GameAnalytics.NewProgressionEvent(status, progress);
                }
            }
        }

        private class DesignEvent : GameAnalyticsEvent
        {
            public string eventName;
            public float? eventValue;

            public override void Track()
            {
                if (eventValue != null) {
                    GameAnalytics.NewDesignEvent(eventName, (float)eventValue);
                } else {
                    GameAnalytics.NewDesignEvent(eventName);
                }
            }
        }
    }
}