﻿using System.Collections.Generic;
 using Voodoo.Sauce.Internal.Analytics;

 namespace Voodoo.Analytics
{
    public static class VoodooAnalyticsManager
    {
        private const string TAG = "AnalyticsManager";

        private static bool _isInitialized;

        private static readonly List<QueuedEvent> QueuedEvents = new List<QueuedEvent>();

        private static readonly Dictionary<AnalyticParameters, object> AnalyticsParameters = new Dictionary<AnalyticParameters, object>();

        /// <summary>
        ///   <param>Add parameters to Analytics before init it.</param>
        /// </summary>
        /// <param name="sessionParameters">List of parameters</param>
        public static void AddSessionParameters(Dictionary<AnalyticParameters, object> sessionParameters)
        {
            foreach (KeyValuePair<AnalyticParameters, object> parameter in sessionParameters) {
                if (!AnalyticsParameters.ContainsKey(parameter.Key)) {
                    AnalyticsParameters.Add(parameter.Key, parameter.Value);
                }
            }
        }

        /// <summary>
        ///   <param>Add parameter to Analytics before init it.</param>
        /// </summary>
        /// <param name="sessionParameter">Parameter key</param>
        /// <param name="value">Value of this key</param>
        public static void AddSessionParameter(AnalyticParameters sessionParameter, object value)
        {
            if (!AnalyticsParameters.ContainsKey(sessionParameter)) {
                AnalyticsParameters.Add(sessionParameter, value);
            }
        }

        /// <summary>
        ///   <param>Init Analytics Manager.</param>
        /// </summary>
        /// <param name="config">Configuration</param>
        public static void Init(IConfig config)
        {
            if (_isInitialized) {
                return;
            }

            _isInitialized = true;

            Tracker.Instance.Init(config);

            SendQueuedEvents();
        }

        private static void SendQueuedEvents()
        {
            QueuedEvents.ForEach(queuedEvent => {
                InternalTrackEvent(queuedEvent.EventName,
                    queuedEvent.EventDataJson,
                    queuedEvent.EventType,
                    queuedEvent.EventCustomVariablesJson);
            });

            QueuedEvents.Clear();
        }

        /// <summary>
        ///   <param>Track an event (list in EventName)</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="data">Internal data</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        public static void TrackEvent(EventName eventName,
                                      Dictionary<string, object> data,
                                      string eventType = null,
                                      Dictionary<string, object> customVariables = null)
        {
            TrackEvent(eventName.ToString(), data, eventType, customVariables);
        }

        /// <summary>
        ///   <param>Track custom  event (not list in EventName)</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        public static void TrackCustomEvent(string eventName,
                                            Dictionary<string, object> customVariables,
                                            string eventType = null)
        {
            TrackEvent(eventName, null, eventType, customVariables);
        }

        /// <summary>
        ///   <param>Track an event</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="data">Internal data</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        private static void TrackEvent(string eventName,
                                       Dictionary<string, object> data,
                                       string eventType = null,
                                       Dictionary<string, object> customVariables = null)
        {
            string dataJson = null;
            if (data != null) {
                dataJson = AnalyticsUtil.ConvertDictionaryToJson(data);
            }

            string customVariablesJson = null;
            if (customVariables != null) {
                customVariablesJson = AnalyticsUtil.ConvertDictionaryToCustomVarJson(customVariables);
            }

            InternalTrackEvent(eventName, dataJson, eventType, customVariablesJson);
        }

        /// <summary>
        ///   <param>Track an event (in EventName list)</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="data">Internal data</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        public static void TrackEvent(EventName eventName,
                                      string data = null,
                                      string eventType = null,
                                      Dictionary<string, object> customVariables = null)
        {
            string customVariablesJson = null;
            if (customVariables != null) {
                customVariablesJson = AnalyticsUtil.ConvertDictionaryToCustomVarJson(customVariables);
            }

            InternalTrackEvent(eventName.ToString(), data, eventType, customVariablesJson);
        }

        private static void InternalTrackEvent(string eventName,
                                               string dataJson,
                                               string eventType,
                                               string customVariablesJson)
        {
            if (!_isInitialized) {
                var queuedEvent = new QueuedEvent {
                    EventName = eventName,
                    EventDataJson = dataJson,
                    EventType = eventType,
                    EventCustomVariablesJson = customVariablesJson
                };
                AnalyticsLog.Log(TAG, "Add event " + eventName + " to the queue (" + dataJson + ")");
                QueuedEvents.Add(queuedEvent);
                return;
            }

            AnalyticsSessionHelper.DefaultHelper().OnNewEvent();

            AnalyticsLog.Log(TAG, "Create event " + eventName + " (" + dataJson + ")");
            Event.Create(eventName,
                AnalyticsParameters,
                dataJson,
                customVariablesJson,
                eventType,
                AnalyticsSessionHelper.DefaultHelper().SessionId,
                AnalyticsSessionHelper.DefaultHelper().SessionLength,
                AnalyticsSessionHelper.DefaultHelper().SessionCount,
                async e => { await Tracker.Instance.TrackEvent(e); });
        }

        private struct QueuedEvent
        {
            public string EventName;
            public string EventDataJson;
            public string EventType;
            public string EventCustomVariablesJson;
        }
    }
}