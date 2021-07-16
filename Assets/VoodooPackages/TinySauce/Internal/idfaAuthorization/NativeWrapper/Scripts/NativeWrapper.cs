using System;
using System.Runtime.InteropServices;
using AOT;

namespace Voodoo.Sauce.Internal.IdfaAuthorization
{
    internal class NativeWrapper
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern int _authorizationStatus();

        private static IdfaAuthorizationStatus CastIntToAuthorizationStatus(int intStatus)
        {
            var authorizationStatus = IdfaAuthorizationStatus.NotDetermined;
            if (Enum.IsDefined(typeof(IdfaAuthorizationStatus), intStatus)) {
                authorizationStatus = (IdfaAuthorizationStatus)intStatus;
            }
            return authorizationStatus;
        }

        /// <summary>
        /// The authorization status.
        /// </summary>
        /// <returns>
        /// For iOS 14 and above:
        /// - NotDetermined: The user has not yet received an authorization request to authorize access his IDFA (IDFA is zeroed)  
        /// - Restricted: The app could not even request to authorize access his IDFA (IDFA is zeroed)
        /// - Denied: The user denied the authorization request to authorize access his IDFA (IDFA is zeroed)
        /// - Authorized: The user authorized the app tp access his IDFA (IDFA is available)
        ///
        /// For iOS 13 and below:
        /// - Denied: The user enabled the Limit Ad Tracking option (IDFA is zeroed)
        /// - Authorized: The user disabled the Limit Ad Tracking option (IDFA is available)
        /// </returns>
        internal static IdfaAuthorizationStatus GetAuthorizationStatus()
        {
#if UNITY_EDITOR
            return IdfaAuthorizationStatus.Authorized;
#else
            return CastIntToAuthorizationStatus(_authorizationStatus());
#endif  
        }   
        
        internal delegate void RequestAuthorizationCallback(IdfaAuthorizationStatus status);
        private delegate void RequestAuthorizationNativeCallback();

        [DllImport("__Internal")]
        private static extern void _requestAuthorization(RequestAuthorizationNativeCallback callback);

        [MonoPInvokeCallback(typeof(RequestAuthorizationNativeCallback))]
        private static void RequestNativeAuthorization(RequestAuthorizationNativeCallback callback)
        {
            _requestAuthorization(callback);
        }
        
        private static RequestAuthorizationCallback _requestAuthorizationCallback;
        [MonoPInvokeCallback(typeof(RequestAuthorizationCallback))]
        private static void RequestAuthorizationCallbackReceived()
        {
            _requestAuthorizationCallback?.Invoke(GetAuthorizationStatus());
            _requestAuthorizationCallback = null;
        }

        /// <summary>
        /// For iOS 14 and above: Request the authorization of the user to access the IDFA, when the AuthorizationStatus is to NotDetermined,
        /// by triggering the Apple one time popup
        ///
        /// For iOS 13 and below: Do nothing
        /// </summary>
        /// <param name="callback">Fired when the user chooses one of Apple popup options: authorize or deny.</param>
        internal static void RequestAuthorization(RequestAuthorizationCallback callback)
        {
#if !UNITY_EDITOR
            _requestAuthorizationCallback += callback;
            RequestNativeAuthorization(RequestAuthorizationCallbackReceived);
#endif
        }
        
        [DllImport("__Internal")]
        private static extern int _redirectToAppSettings();

        /// <summary>
        /// Redirect the user to the Settings app and displays the app’s custom settings, if it has any.
        /// </summary>
        internal static void RedirectToAppSettings()
        {
#if !UNITY_EDITOR
            _redirectToAppSettings();
#endif
        }
        
#endif
    }
}