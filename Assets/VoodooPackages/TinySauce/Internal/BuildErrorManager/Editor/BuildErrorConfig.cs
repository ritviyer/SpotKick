
using System.Collections.Generic;

namespace Voodoo.Sauce.Internal.Editor
{
    public static class BuildErrorConfig
    {
        // Individual offsets by error type 
         private const int no = 0;
         private const int ga = 300;

         public enum ErrorID
         {
             NoVoodooSettings = no,
             SettingsNoFacebookAppID,
             GANoIOSKey = ga, 
             GANoAndroidAndKey,
             NoAdjustToken,
             INVALID_PLATFORM
         }
         
         public  static readonly Dictionary<ErrorID, string> ErrorMessageDict = new Dictionary<ErrorID, string>
         {
             {ErrorID.NoVoodooSettings, "No Settings file found.  Check your path for Assets/Resources/TinySauce/Settings.asset"},
             {ErrorID.INVALID_PLATFORM, "Invalid Platform please switch to IOS or Android on your Build Settings"},
             {ErrorID.SettingsNoFacebookAppID, "TinySauce Settings is missing Facebook App Id"},
             {ErrorID.GANoIOSKey, "TinySauce Settings is missing iOS GameAnalytics keys"},
             {ErrorID.GANoAndroidAndKey, "TinySauce Settings is missing Android GameAnalytics keys! add 'ignore' in both fields to disable Android analytics"},
             {ErrorID.NoAdjustToken, "TinySauce Settings is missing Adjust App Token"}
         };
    }
 }
