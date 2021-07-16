using System.Collections.Generic;
using Voodoo.Analytics;
//using Voodoo.Sauce.Internal.Ads;
//using Voodoo.Sauce.Internal.RemoteConfig;

namespace Voodoo.Sauce.Internal.Analytics
{
    internal class VoodooAnalyticsProvider : IAnalyticsProvider
    {
        private readonly VoodooAnalyticsParameters _parameters;

        internal VoodooAnalyticsProvider(VoodooAnalyticsParameters parameters)
        {
            
            _parameters = parameters;
            
            if (!_parameters.UseVoodooAnalytics) return;
            RegisterEvents();
        }

        public void Initialize(bool consent)
        {
            if (!_parameters.UseVoodooAnalytics) return;
            AnalyticsConfig analyticsConfig = new AnalyticsConfig();

            bool useRemoteConfig = _parameters.UseRemoteConfig;
            VoodooAnalyticsManager.AddSessionParameters(new Dictionary<AnalyticParameters, object> {
               {AnalyticParameters.VoodooSauceVersion, TinySauce.Version+"_TS"},
                {AnalyticParameters.SegmentationUuid, useRemoteConfig ? "" : null},
                {AnalyticParameters.AbTestUuid, useRemoteConfig ? "" : null},
                {AnalyticParameters.AbTestCohortUuid, useRemoteConfig ? "" : ""},
                {AnalyticParameters.AbTestVersionUuid, useRemoteConfig ? "" : null}
#if UNITY_EDITOR
                , {AnalyticParameters.EditorIdfa, _parameters.EditorIdfa}
#endif
            });
            
            VoodooAnalyticsManager.Init(analyticsConfig);
        }

        private static void RegisterEvents()
        {
            AnalyticsManager.OnApplicationFirstLaunchEvent += OnApplicationFirstLaunch;
            AnalyticsManager.OnApplicationLaunchEvent += OnApplicationLaunchEvent;
            AnalyticsManager.OnGameStartedEvent += OnGameStarted;
            AnalyticsManager.OnGameFinishedEvent += OnGameFinished;
            AnalyticsManager.OnTrackCustomEvent += TrackCustomEvent;
            //AnalyticsManager.OnRewardedVideoButtonShownEvent += OnRewardedVideoButtonShownEvent;
            // Activate this line if we want to track purchase without partner verification
            //VoodooAnalytics.OnTrackPurchaseEvent += OnTrackPurchaseEvent;
            //AnalyticsManager.OnTrackPurchaseVerificationEvent += OnTrackPurchaseVerificationEvent;
            //AnalyticsManager.OnNoAdsClickEvent += OnNoAdsClickEvent;

            //AnalyticsManager.OnCrossPromoShownEvent += OnCrossPromoShownEvent;
            //AnalyticsManager.OnCrossPromoClickEvent += OnCrossPromoClickEvent;

            //AnalyticsManager.OnBannerShownEvent += (adUnit, height) => VoodooAnalyticsManager.TrackEvent(EventName.banner_shown);
            //AnalyticsManager.OnBannerClickedEvent += adUnit => VoodooAnalyticsManager.TrackEvent(EventName.banner_click);
            //AnalyticsManager.OnInterstitialClickedEvent += OnInterstitialClickedEvent;
            //AnalyticsManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
            //AnalyticsManager.OnShowInterstitialEvent += OnShowInterstitialEvent;
           // AnalyticsManager.OnRewardedVideoClickedEvent += OnRewardedVideoClickedEvent;
            //AnalyticsManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
            //AnalyticsManager.OnShowRewardedVideoEvent += OnShowRewardedVideoEvent;
            //AnalyticsManager.OnImpressionTrackedEvent +=
                //(adUnit, impressionData) => VoodooAnalyticsManager.TrackEvent(EventName.ad_revenue, impressionData);
        }

        //private static void OnNoAdsClickEvent()
        //{
        //    VoodooAnalyticsManager.TrackEvent(EventName.noads_click);
        //}

        //private static void OnInterstitialClickedEvent(string interstitialType, string adUnit)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.INTERSTITIAL_TYPE, interstitialType},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.fs_click, data);
        //}

        //private static void OnShowInterstitialEvent(string interstitialType, int rvCount)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.INTERSTITIAL_TYPE, interstitialType},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.fs_shown, data);
        //}

        //private static void OnInterstitialDismissedEvent(string interstitialType, string adUnits)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.INTERSTITIAL_TYPE, interstitialType},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.fs_watched, data);
        //}

        //private static void OnRewardedVideoClickedEvent(string rewardedType, string adUnit)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.REWARDED_TYPE, rewardedType},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.rv_click, data);
        //}

        //private static void OnShowRewardedVideoEvent(string rewardedType, int rvCount)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.REWARDED_TYPE, rewardedType},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.rv_shown, data);
        //}

        //private static void OnRewardedVideoClosedEvent(string rewardedType, string adUnits)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.REWARDED_TYPE, rewardedType},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.rv_watched, data);
        //}

        //private static void OnRewardedVideoButtonShownEvent(string rewardedType)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.REWARDED_TYPE, rewardedType},
        //        {VoodooAnalyticsConstants.REWARDED_LOADED, Voodoo.Sauce.Internal.Ads.AdsManager.IsRewardedVideoAvailable()},
        //        {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}
        //    };

        //    VoodooAnalyticsManager.TrackEvent(EventName.rv_trigger, data);
        //}

        //private static void OnCrossPromoShownEvent(CrossPromoAnalyticsInfo crossPromoInfo)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.BUNDLE_ID, crossPromoInfo.gameBundleId},
        //        {VoodooAnalyticsConstants.FILE_PATH, crossPromoInfo.assetPath}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.cp_impression, data);
        //}

        //private static void OnCrossPromoClickEvent(CrossPromoAnalyticsInfo crossPromoInfo)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.BUNDLE_ID, crossPromoInfo.gameBundleId},
        //        {VoodooAnalyticsConstants.FILE_PATH, crossPromoInfo.assetPath}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.cp_click, data);
        //}

        private static void OnApplicationFirstLaunch()
        {
            VoodooAnalyticsManager.TrackEvent(EventName.app_install);
        }

        private static void OnApplicationLaunchEvent()
        {
            VoodooAnalyticsManager.TrackEvent(EventName.app_open);
        }

        /*private static void OnTrackPurchaseEvent(AnalyticsPurchaseInfo purchaseInfo)
        {
            var data = new Dictionary<string, object> {
                {VoodooAnalyticsConstants.IAP_ID, purchaseInfo.productId},
                {VoodooAnalyticsConstants.PRICE, (float) purchaseInfo.localizedPrice},
                {VoodooAnalyticsConstants.CURRENCY, purchaseInfo.isoCurrencyCode},
            };
            VoodooAnalyticsManager.TrackEvent(EventName.iap, data);
        }*/

        //private static void OnTrackPurchaseVerificationEvent(PurchaseAnalyticsInfo purchaseInfo, bool validated)
        //{
        //    var data = new Dictionary<string, object> {
        //        {VoodooAnalyticsConstants.IAP_ID, purchaseInfo.productId},
        //        {VoodooAnalyticsConstants.PRICE, (float) purchaseInfo.localizedPrice},
        //        {VoodooAnalyticsConstants.CURRENCY, purchaseInfo.isoCurrencyCode},
        //        {VoodooAnalyticsConstants.VALIDATED, validated}
        //    };
        //    VoodooAnalyticsManager.TrackEvent(EventName.iap, data);
        //}

        private static void OnGameStarted(string level, Dictionary<string, object> eventProperties)
        {
            var data = new Dictionary<string, object> {
                {VoodooAnalyticsConstants.LEVEL, level},
                {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()},
                {VoodooAnalyticsConstants.HIGHEST_SCORE, AnalyticsStorageHelper.GetGameHighestScore()}
            };
            VoodooAnalyticsManager.TrackEvent(EventName.game_start, data, null, eventProperties);
        }

        private static void OnGameFinished(bool levelComplete, float score, string level, Dictionary<string, object> eventProperties)
        {
            var data = new Dictionary<string, object> {
                {VoodooAnalyticsConstants.LEVEL, level},
                {VoodooAnalyticsConstants.GAME_COUNT, AnalyticsStorageHelper.GetGameCount()},
                {VoodooAnalyticsConstants.STATUS, levelComplete},
                {VoodooAnalyticsConstants.SCORE, score}
            };
            VoodooAnalyticsManager.TrackEvent(EventName.game_finish, data, null, eventProperties);
        }

        private static void TrackCustomEvent(string eventName,
                                             Dictionary<string, object> customVariables,
                                             string eventType,
                                             List<TinySauce.AnalyticsProvider> analyticsProviders)
        {
            if (analyticsProviders.Contains(TinySauce.AnalyticsProvider.VoodooAnalytics)) {
                VoodooAnalyticsManager.TrackCustomEvent(eventName, customVariables, eventType);
            }
        }
    }
}