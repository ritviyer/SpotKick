namespace Voodoo.Sauce.Internal.Analytics
{
    public class VoodooAnalyticsParameters
    {
        public readonly bool UseRemoteConfig;
        public readonly bool UseVoodooAnalytics;
        public readonly string EditorIdfa;

        public VoodooAnalyticsParameters(bool useRemoteConfig, bool useVoodooAnalytics, string editorIdfa)
        {
            UseRemoteConfig = useRemoteConfig;
            UseVoodooAnalytics = useVoodooAnalytics;
            EditorIdfa = editorIdfa;
        }
    }
}