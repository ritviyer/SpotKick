using com.adjust.sdk;
using UnityEngine;
using UnityEngine.UI;
using Voodoo.Sauce.Internal.IdfaAuthorization;

namespace Voodoo.Sauce.Internal
{
    public class TSDebugUIScreenInfo : TSDebugUIScreen
    {
        [Header("== App Info Fields ==")]
        [SerializeField] private Text appName;
        [SerializeField] private Text appBundleID;
        [SerializeField] private Text appVersion;

        [Header("== TinySauce Info Fields ==")]
        [SerializeField] private Text gaTitle;
        [SerializeField] private Text gaIosGameKey;
        [SerializeField] private Text gaIosSecretKey;
        [SerializeField] private Text gaAndroidGameKey;
        [SerializeField] private Text gaAndroidSecretKey;
        [Space(4)]
        [SerializeField] private Text fbTitle;
        [SerializeField] private Text fbAppID;
        [Space(4)]
        [SerializeField] private Text adjustTitle;
        [SerializeField] private Text adjustIosToken;
        [SerializeField] private Text adjustAndroidToken;

        [Header("== Device Info Fields ==")]
        [SerializeField] private Text idfaTitle;
        [SerializeField] private Text idfaStatusTitle;
        [SerializeField] private Text idfaIDTitle;
        [SerializeField] private Text idfaStatus;
        [SerializeField] private Text idfaID;

        private string notDeterminedText = "Not determined";

        private bool googleAAIDStatus = false;
        private string googleAAID = "";


        protected override void UpdateInfo()
        {
            base.UpdateInfo();

            appName.text = Application.productName;
            appBundleID.text = Application.identifier;
            appVersion.text = Application.version;

            gaTitle.text = "GameAnalytics - v. " + GameAnalyticsSDK.Setup.Settings.VERSION;
            gaIosGameKey.text = TSSettings.gameAnalyticsIosGameKey;
            gaIosSecretKey.text = TSSettings.gameAnalyticsIosSecretKey;
            gaAndroidGameKey.text = TSSettings.gameAnalyticsAndroidGameKey;
            gaAndroidSecretKey.text = TSSettings.gameAnalyticsAndroidSecretKey;

            fbTitle.text = "Facebook - v. " + Facebook.Unity.FacebookSdkVersion.Build;
            fbAppID.text = TSSettings.facebookAppId;

            adjustTitle.text = (Adjust.getSdkVersion() == null || Adjust.getSdkVersion() == "") ? "Adjust" : "Adjust - v. " + Adjust.getSdkVersion().Split("@"[0])[1]; //get Adjust version and remove useless info before the "@"
            adjustIosToken.text = TSSettings.adjustIOSToken;
            adjustAndroidToken.text = TSSettings.adjustAndroidToken;

#if UNITY_IOS
            idfaStatus.text = NativeWrapper.GetAuthorizationStatus().ToString();
            idfaID.text = NativeWrapper.GetAuthorizationStatus() == IdfaAuthorizationStatus.Authorized ? Adjust.getIdfa() : notDeterminedText;

    #if UNITY_EDITOR
            idfaID.text = NativeWrapper.GetAuthorizationStatus() == IdfaAuthorizationStatus.Authorized ? TSSettings.EditorIdfa : notDeterminedText;
    #endif
#elif UNITY_ANDROID
            idfaTitle.text = " Google AAID";
            idfaStatusTitle.text = "  AAID Status:";
            idfaIDTitle.text = "  AAID:";

            if (googleAAID == "")
            {
                Application.RequestAdvertisingIdentifierAsync((advertisingId, adTrackingEnabled, error) =>
                {
                    googleAAIDStatus = adTrackingEnabled;
                    googleAAID = googleAAIDStatus ? advertisingId : notDeterminedText;

                    UpdateGoogleAdId();
                });
            }
            else
                UpdateGoogleAdId();

    #if UNITY_EDITOR
            idfaStatus.text = IdfaAuthorizationStatus.Authorized.ToString();
            idfaID.text = TSSettings.EditorIdfa;
    #endif
#endif
        }

        private void UpdateGoogleAdId()
        {
            idfaStatus.text = googleAAIDStatus ? IdfaAuthorizationStatus.Authorized.ToString() : IdfaAuthorizationStatus.Denied.ToString();
            idfaID.text = googleAAID;
        }
    }
}
