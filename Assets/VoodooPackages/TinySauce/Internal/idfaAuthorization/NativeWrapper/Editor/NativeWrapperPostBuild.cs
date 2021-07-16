#if UNITY_IOS || UNITY_TVOS
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using Voodoo.Sauce.Internal.Utils;

namespace Voodoo.Sauce.Internal.IdfaAuthorization
{
    public class NativeWrapperPostBuild
    {
        private const string InfoPlistStringsFileName = "InfoPlist.strings";
        private const string UserTrackingUsageDescriptionKey = "NSUserTrackingUsageDescription";
       

        [PostProcessBuild(1000)]
        public static void PostprocessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (IdfaAuthorizationUtils.IsAttEnabled())
            {
                if (buildTarget != BuildTarget.iOS)
                    return;


                var plistPath = Path.Combine(buildPath, "Info.plist");
                var plist = new PlistDocument();
                plist.ReadFromFile(plistPath);
                PlistElement element = plist.root[UserTrackingUsageDescriptionKey];

              
                PlistElementArray skAdNetworks = plist.root.CreateArray("SKAdNetworkItems");
                
                PlistElementDict fbSkanId = skAdNetworks.AddDict();
                fbSkanId.SetString("SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork");
                PlistElementDict fbSkanSecondaryId = skAdNetworks.AddDict();
                fbSkanSecondaryId.SetString("SKAdNetworkIdentifier", "n38lu8286q.skadnetwork");
                PlistElementDict snapSkanId = skAdNetworks.AddDict();
                snapSkanId.SetString("SKAdNetworkIdentifier", "424m5254lk.skadnetwork");
                

                plist.root.SetString(UserTrackingUsageDescriptionKey, IdfaAuthorizationConstants.UserTrackingUsageDescription);
                plist.WriteToFile(plistPath);

            }
        }

       
    }
}
#endif