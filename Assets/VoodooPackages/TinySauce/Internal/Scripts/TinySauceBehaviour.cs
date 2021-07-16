using System;
using UnityEngine;
using Voodoo.Sauce.Internal.Analytics;
using Voodoo.Sauce.Internal.IdfaAuthorization;
using Facebook.Unity;

namespace Voodoo.Sauce.Internal
{
    internal class TinySauceBehaviour : MonoBehaviour
    {
        private const string TAG = "TinySauce";
        private static TinySauceBehaviour _instance;
        private TinySauceSettings _sauceSettings;


        private void Awake()
        {
    
            #if UNITY_IOS

                NativeWrapper.RequestAuthorization((status) =>
                {
                    InitFacebook();
                });

            #elif UNITY_ANDROID

                InitFacebook();

            #endif
            
            if (transform != transform.root)
                throw new Exception("TinySauce prefab HAS to be at the ROOT level!");

            _sauceSettings = TinySauceSettings.Load();
            if (_sauceSettings == null)
                throw new Exception("Can't find TinySauce sauceSettings file.");
            
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);
            
            VoodooLog.Initialize(VoodooLogLevel.WARNING);
            // init TinySauce sdk
            InitAnalytics();
        }
        

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        private void InitAnalytics()
        {
            VoodooLog.Log(TAG, "Initializing Analytics");
            
            AnalyticsManager.Initialize(_sauceSettings, true);
            
        }
        private void InitFacebook()
        {
            if (!FB.IsInitialized) FB.Init(InitCallback, OnHideUnity);
            
            else FB.ActivateApp();           
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // Brought forward after soft closing 
            // Brought forward after soft closing 
            if (!pauseStatus) {
                AnalyticsManager.OnApplicationResume();
            }
        }
    }
}