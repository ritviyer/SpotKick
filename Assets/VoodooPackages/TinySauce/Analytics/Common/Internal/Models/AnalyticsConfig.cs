using System;
using Voodoo.Analytics;

namespace Voodoo.Sauce.Internal.Analytics
{
    [Serializable]
    public class AnalyticsConfig : IConfig
    {
        public int waitIntervalSeconds = 5;
        public int maxNumberOfEventsPerFile = 50;
        public string enabledEvents = "";
        public static int sessionIdRenewalIntervalInSeconds = 300;

        public int GetSenderWaitIntervalSeconds()
        {
            return waitIntervalSeconds;
        }

        public int GetMaxNumberOfEventsPerFile()
        {
            return maxNumberOfEventsPerFile;
        }

        public string[] EnabledEvents()
        {
            if (String.IsNullOrEmpty(enabledEvents))
            {
                return new string[] { };
            }

            return enabledEvents.Split('|');
        }

        public int SessionIdRenewalIntervalInSeconds()
        {
            return sessionIdRenewalIntervalInSeconds;
        }

        public override string ToString()
        {
            var enabledEventsString = "all";
            if (EnabledEvents().Length > 0)
            {
                enabledEventsString = string.Join(", ", EnabledEvents());
            }

            return "- wait interval: " + waitIntervalSeconds +
                "\n- max number of events per file: " + maxNumberOfEventsPerFile +
                "\n- session id renewal interval: " + sessionIdRenewalIntervalInSeconds +
                "\n- enabled events: " + enabledEventsString;
        }
    }
}