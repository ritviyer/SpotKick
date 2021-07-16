﻿namespace Voodoo.Analytics
{
    public interface IConfig
    {
        int GetSenderWaitIntervalSeconds();

        int GetMaxNumberOfEventsPerFile();

        string[] EnabledEvents();

        int SessionIdRenewalIntervalInSeconds();
    }
}