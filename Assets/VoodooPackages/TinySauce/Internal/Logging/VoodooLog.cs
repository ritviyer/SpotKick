using System;
using UnityEngine;

namespace Voodoo.Sauce.Internal
{
    public static class VoodooLog
    {
        private static VoodooLogLevel _logLevel;
        private const string TAG = "TinySauce";
        
        public static void Initialize(VoodooLogLevel level)
        {
            _logLevel = level;
            EnableLogs();
        }

        public static void Log(string tag, string message)
        {
            if (!Application.isEditor || _logLevel >= VoodooLogLevel.DEBUG)
                Debug.Log(Format(tag, message));
        }

        public static void LogE(string tag, string message)
        {
            if (!Application.isEditor || _logLevel >= VoodooLogLevel.ERROR)
                Debug.LogError(Format(tag, message));
        }

        public static void LogW(string tag, string message)
        {
            if (!Application.isEditor || _logLevel >= VoodooLogLevel.WARNING)
                Debug.LogWarning(Format(tag, message));
        }
        
        private static string Format(string tag, string message) => $"{DateTime.Now} - {TAG}/{tag}: {message}";

        // Separate Log enable/disable separate from VoodooLogLevel 
        public static void DisableLogs()
        {
            Debug.unityLogger.logEnabled = false;
        }

        private static void EnableLogs()
        {
            Debug.unityLogger.logEnabled = true;
        }

        public static bool IsLogsEnabled() => Debug.unityLogger.logEnabled;
    }

    public enum VoodooLogLevel
    {
        ERROR=0,
        WARNING=1,
        DEBUG=2
    }
}