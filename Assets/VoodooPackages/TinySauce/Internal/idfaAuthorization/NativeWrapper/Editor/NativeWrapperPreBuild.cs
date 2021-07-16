using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Voodoo.Sauce.Internal.IdfaAuthorization;


namespace Voodoo.Sauce.Internal.IdfaAuthorization
{
    public class NativeWrapperPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateAppTrackingTransparencySymbol();
        }
        
        private void UpdateAppTrackingTransparencySymbol()
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            
            bool isAttEnabled = IdfaAuthorizationUtils.IsAttEnabled();
            bool isAttSymbolPresent = symbols.Contains(IdfaAuthorizationConstants.AppTrackingTransparencySymbol);
            
            if (isAttEnabled && !isAttSymbolPresent) {
                symbols = IdfaAuthorizationConstants.AppTrackingTransparencySymbol + ";" + symbols;
            }
            else if (!isAttEnabled && isAttSymbolPresent) {
                symbols = Regex.Replace(symbols, IdfaAuthorizationConstants.AppTrackingTransparencySymbol, "");
                symbols = Regex.Replace(symbols, ";;", ";");
            }
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, symbols);
        }
        
    }
}