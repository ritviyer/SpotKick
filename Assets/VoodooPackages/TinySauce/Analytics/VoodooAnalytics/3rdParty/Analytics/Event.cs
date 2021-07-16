﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 using Voodoo.Sauce.Internal.Analytics;
 using Voodoo.Sauce.Internal.IdfaAuthorization;

#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace Voodoo.Analytics
{
    internal class Event
    {

        private const string EventTypeImpression = "impression";
        private const string EventTypeApp = "app";
        private const string EventTypeCustom = "custom";

        private Dictionary<string, object> _values;
        private string _name;
        private string _jsonData;
        private string _customVariablesData;

        internal string GetName() => _name;

        private static readonly string[] ImpressionEvents = {
            EventName.fs_shown.ToString(),
            EventName.fs_click.ToString(),
            EventName.fs_watched.ToString(),
            EventName.rv_shown.ToString(),
            EventName.rv_click.ToString(),
            EventName.rv_watched.ToString(),
            EventName.rv_trigger.ToString(),
            EventName.banner_shown.ToString(),
            EventName.banner_click.ToString(),
            EventName.ad_revenue.ToString(),
            EventName.cp_impression.ToString(),
            EventName.cp_click.ToString()
        };

        private Event() { }

        internal static void Create(string name,
                                  Dictionary<AnalyticParameters, object> parameters,
                                  string dataJson,
                                  string customVariablesJson,
                                  string type,
                                  string sessionId,
                                  int sessionLength,
                                  int sessionCount,
                                  Action<Event> complete)
        {
            var eventValues = new Dictionary<string, object> {
                {AnalyticsConstant.VS_VERSION, parameters[AnalyticParameters.VoodooSauceVersion]?.ToString()},
                {AnalyticsConstant.ID, Guid.NewGuid().ToString()},
                {AnalyticsConstant.USER_ID, AnalyticsUserIdHelper.GetUserId()},
                {AnalyticsConstant.NAME, name},
                {AnalyticsConstant.TYPE, type ?? GetType(name)},
                {AnalyticsConstant.CREATED_AT, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")},
                {AnalyticsConstant.BUNDLE_ID, Application.identifier},
                {AnalyticsConstant.APP_VERSION, Application.version},
                {AnalyticsConstant.LOCALE, GetLocale()},
                {AnalyticsConstant.CONNECTIVITY, GetConnectivity()},
                {AnalyticsConstant.SESSION_ID, sessionId},
                {AnalyticsConstant.SESSION_LENGTH, sessionLength},
                {AnalyticsConstant.SESSION_COUNT, sessionCount},
#if UNITY_EDITOR
                {AnalyticsConstant.PLATFORM, "editor"},
                {AnalyticsConstant.DEVICE, SystemInfo.deviceModel},
                {AnalyticsConstant.OS_VERSION, SystemInfo.operatingSystem},
                {AnalyticsConstant.DEVELOPER_DEVICE_ID, ""},
                {AnalyticsConstant.ADVERTISING_ID, parameters[AnalyticParameters.EditorIdfa]?.ToString()},
                {AnalyticsConstant.LIMIT_AD_TRACKING, true},
#elif UNITY_ANDROID
                {AnalyticsConstant.PLATFORM, "android"},
                {AnalyticsConstant.DEVICE, SystemInfo.deviceModel},
                {AnalyticsConstant.OS_VERSION, SystemInfo.operatingSystem},
                {AnalyticsConstant.DEVELOPER_DEVICE_ID, ""},
#elif UNITY_IOS
                {AnalyticsConstant.PLATFORM, "ios"},
                {AnalyticsConstant.DEVICE, Device.generation.ToString()},
                {AnalyticsConstant.OS_VERSION, Device.systemVersion},
                {AnalyticsConstant.DEVELOPER_DEVICE_ID, Device.vendorIdentifier.Replace("-", "").ToLower()},
                {AnalyticsConstant.IDFA_AUTHORIZATION_STATUS, NativeWrapper.GetAuthorizationStatus().ToString()},
#endif
            };

            if (ParameterHasValue(parameters, AnalyticParameters.SegmentationUuid))
            {
                eventValues.Add(AnalyticsConstant.SEGMENT_UUID, parameters[AnalyticParameters.SegmentationUuid].ToString());                
            }
            
            if (ParameterHasValue(parameters, AnalyticParameters.AbTestUuid))
            {
                eventValues.Add(AnalyticsConstant.AB_TEST_UUID, parameters[AnalyticParameters.AbTestUuid].ToString());                
            }
            
            if (ParameterHasValue(parameters, AnalyticParameters.AbTestCohortUuid))
            {
                eventValues.Add(AnalyticsConstant.COHORT_UUID, parameters[AnalyticParameters.AbTestCohortUuid].ToString());                
            }

            if (ParameterHasValue(parameters, AnalyticParameters.AbTestVersionUuid))
            {
                eventValues.Add(AnalyticsConstant.VERSION_UUID, parameters[AnalyticParameters.AbTestVersionUuid].ToString());                
            }
            

#if UNITY_EDITOR
            complete(new Event {_values = eventValues, _name = name, _jsonData = dataJson, _customVariablesData = customVariablesJson});
#else
            AnalyticsSessionHelper.GetAdvertisingId(delegate(string advertisingId, bool limitAdTracking) {
                eventValues.Add(AnalyticsConstant.ADVERTISING_ID, advertisingId);
                eventValues.Add(AnalyticsConstant.LIMIT_AD_TRACKING, limitAdTracking);
#if UNITY_ANDROID
                eventValues.Add(AnalyticsConstant.IDFA_AUTHORIZATION_STATUS, limitAdTracking ? IdfaAuthorizationStatus.Authorized.ToString() : IdfaAuthorizationStatus.Authorized.ToString());
#endif 
                complete(new Event {_values = eventValues, _name = name, _jsonData = dataJson, _customVariablesData = customVariablesJson});
            });
#endif
        }

        private static bool ParameterHasValue(Dictionary<AnalyticParameters, object> parameters,
                                       AnalyticParameters parameterKey) => !string.IsNullOrEmpty(parameters[parameterKey]?.ToString());

        private static string GetType(string name)
        {
            string type;
            if (!Enum.IsDefined(typeof(EventName), name)) {
                type = EventTypeCustom;
            } else if (ImpressionEvents.Contains(name)) {
                type = EventTypeImpression;
            } else {
                type = EventTypeApp;
            }

            return type;
        }

        internal string ToJson()
        {
            return AnalyticsUtil.ConvertDictionaryToJson(_values, _jsonData, _customVariablesData);
        }

        private static string GetConnectivity()
        {
            switch (Application.internetReachability) {
                case NetworkReachability.NotReachable:
                    return "Offline";
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return "Network";
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return "Wifi";
                default:
                    return "unknown";
            }
        }

        public override string ToString()
        {
            return _values.ToString();
        }

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern string _getNativeLocale();
#endif
        private static string GetLocale()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return _getNativeLocale();
#elif UNITY_ANDROID && !UNITY_EDITOR
            var locale = new AndroidJavaClass("java.util.Locale");
            var defautLocale = locale.CallStatic<AndroidJavaObject>("getDefault");
            var country = defautLocale.Call<string>("getCountry");
            return country;
#else
            return "us";
#endif
        }
    }
}