using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.Sauce.Internal
{
    public class TSDebugUIScreenEvents : TSDebugUIScreen
    {
        [SerializeField] private Text feedbackText;
        [SerializeField] private float feedbackTextDisplayDuration = 1.5f;


        private List<TinySauce.AnalyticsProvider> analyticsProviderList;
        private Coroutine currentCoroutine;


        private void Awake()
        {
            analyticsProviderList = new List<TinySauce.AnalyticsProvider>();
            analyticsProviderList.Add(TinySauce.AnalyticsProvider.GameAnalytics);
            analyticsProviderList.Add(TinySauce.AnalyticsProvider.VoodooAnalytics);

            feedbackText.gameObject.SetActive(false);
        }


        public void Send_StartEvent()
        {
            TinySauce.OnGameStarted("(!)_START_EVENT_DEBUG_(!)");
            DisplayFeedbackText("Start event sent successfully.");
        }

        public void Send_FinishEvent()
        {
            TinySauce.OnGameFinished(true, 999999, "(!)_FINISH_EVENT_DEBUG_(!)");
            DisplayFeedbackText("Finish event sent successfully.");
        }

        public void Send_CustomEvent()
        {
            TinySauce.TrackCustomEvent("(!)_CUSTOM_EVENT_DEBUG_(!)", null, null, analyticsProviderList);
            DisplayFeedbackText("Custom event sent successfully.");
        }

        private void DisplayFeedbackText(string text)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(Coroutine_DisplayFeedbackText(text));
        }


        private IEnumerator Coroutine_DisplayFeedbackText(string text)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = text;

            yield return new WaitForSecondsRealtime(feedbackTextDisplayDuration);


            feedbackText.gameObject.SetActive(false);
        }
    }
}
