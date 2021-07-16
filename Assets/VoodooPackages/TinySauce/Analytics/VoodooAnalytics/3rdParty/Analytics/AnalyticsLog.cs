using System;
using UnityEngine;

namespace Voodoo.Analytics
{
    public static class AnalyticsLog
    {
        private static AnalyticsLogLevel _logLevel;
        private const string TAG = "AnalyticsLog";

        public static void Initialize(AnalyticsLogLevel level)
        {
            _logLevel = level;
        }

        public static void Log(string tag, string message)
        {
            if (!Application.isEditor || _logLevel >= AnalyticsLogLevel.DEBUG)
                Debug.Log(Format(tag, message));
        }

        public static void LogE(string tag, string message)
        {
            if (!Application.isEditor || _logLevel >= AnalyticsLogLevel.ERROR)
                Debug.LogError(Format(tag, message));
        }

        public static void LogW(string tag, string message)
        {
            if (!Application.isEditor || _logLevel >= AnalyticsLogLevel.WARNING)
                Debug.LogWarning(Format(tag, message));
        }

        private static string Format(string tag, string message)
        {
            return $"{DateTime.Now} - {TAG}/{tag}: {message}";
        }

        public enum AnalyticsLogLevel
        {
            ERROR = 0,
            WARNING = 1,
            DEBUG = 2,
            DISABLED = 3
        }
    }
}