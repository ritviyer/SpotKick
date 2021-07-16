﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
 using static Voodoo.Sauce.Internal.EnvironmentSettings.EnvironmentSettings;

 namespace Voodoo.Analytics
{
    internal static class AnalyticsApi
    {
        private const string TAG = "Analytics - Sender";

        private static readonly string AnalyticsGatewayUrl = GetURL("https://vs-api.voodoo-{0}.io/push-analytics",Api.Analytics);
        
        internal static void SendEvents(List<string> events, Action<bool> complete)
        {
            AnalyticsLog.Log(TAG, "Send " + events.Count + " event(s)");

            var request = new UnityWebRequest(AnalyticsGatewayUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes("[" + string.Join(",", events) + "]");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
            asyncOperation.completed += operation => {
                if (!request.isNetworkError && !request.isHttpError) {
                    AnalyticsLog.Log(TAG, "Successfully pushed " + events.Count + " events");
                    complete(true);
                } else {
                    complete(false);
                    AnalyticsLog.Log(TAG, "Error when sending events: " + request.error);
                }

                request.Dispose();
            };
        }
    }
}