using System;
using System.Timers;
using UnityEngine;
using Voodoo.Sauce.Internal.PrivacyHelpers;
//using Voodoo.Sauce.Internal.RemoteConfig;

namespace Voodoo.Sauce.Internal.Analytics
{
    internal class AnalyticsSessionHelper
    {
        private const string TAG = "Analytics - SessionHelper";

        private const string PlayerPrefSessionCountKey = "VOODOO_ANALYTICS_SESSION_COUNT";

        private DateTime _lastEventCreationDate = DateTime.Now;
        private System.Timers.Timer _sessionLengthTimer;
        private int _sessionIdRenewalIntervalInSeconds;

        private static string _advertisingId;
        private static bool _trackingEnabled;

        internal int SessionLength { get; private set; }
        internal string SessionId { get; private set; }
        internal int SessionCount { get; private set; } = -1;

        private static AnalyticsSessionHelper _defaultHelper;

        public static AnalyticsSessionHelper DefaultHelper()
        {
            if (_defaultHelper == null)
            {
                _defaultHelper = new AnalyticsSessionHelper();
                _defaultHelper.Init();
            }

            return _defaultHelper;
        }

        internal void Init()
        {
            _sessionIdRenewalIntervalInSeconds = AnalyticsConfig.sessionIdRenewalIntervalInSeconds;
            if (_sessionLengthTimer == null)
            {
                _sessionLengthTimer = new System.Timers.Timer(1000) { Enabled = true, AutoReset = true };
                _sessionLengthTimer.Elapsed += (sender, args) => { SessionLength++; };
                _sessionLengthTimer.Start();
            }

            // no session count value yet => take the injected value
            if (!PlayerPrefs.HasKey(PlayerPrefSessionCountKey))
            {
                PlayerPrefs.SetInt(PlayerPrefSessionCountKey, AnalyticsStorageHelper.GetAppLaunchCount());
            }
            else
            {
                IncrementSessionCount();
            }

            SessionCount = PlayerPrefs.GetInt(PlayerPrefSessionCountKey, 0);

            ResetSession();
            RefreshCreationDate();
        }

        internal void OnNewEvent()
        {
            if (_lastEventCreationDate.AddSeconds(_sessionIdRenewalIntervalInSeconds) < DateTime.Now)
            {
                Update();
            }

            RefreshCreationDate();
        }

        internal delegate void OnAdvertisingIdReceived(string advertisingId, bool limitAdTracking);

        internal static void GetAdvertisingId(OnAdvertisingIdReceived callback)
        {
            if (_advertisingId != null)
            {
                callback?.Invoke(_advertisingId, !_trackingEnabled);
                return;
            }

            IdfaHelper.RequestAdvertisingId(
                (advertisingId, trackingEnabled, error) => {
                    _advertisingId = advertisingId;
                    _trackingEnabled = trackingEnabled;
                    callback?.Invoke(advertisingId, !trackingEnabled);
                }
            , true, false);
        }

        private void Update()
        {
            ResetSession();
            IncrementSessionCount();
            RefreshCreationDate();
        }

        private void ResetSession()
        {
            // renew session id
            SessionId = Guid.NewGuid().ToString();

            // reset session length counter
            SessionLength = 0;

            VoodooLog.Log(TAG, "New session id: " + SessionId);
        }

        private void IncrementSessionCount()
        {
            SessionCount = PlayerPrefs.GetInt(PlayerPrefSessionCountKey, 0) + 1;
            PlayerPrefs.SetInt(PlayerPrefSessionCountKey, SessionCount);
            VoodooLog.Log(TAG, "Session count incremented to: " + SessionCount);
        }

        private void RefreshCreationDate() => _lastEventCreationDate = DateTime.Now;
    }
}