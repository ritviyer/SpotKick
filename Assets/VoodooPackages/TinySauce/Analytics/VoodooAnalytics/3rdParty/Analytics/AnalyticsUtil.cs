using System.Collections.Generic;
using System.Linq;

namespace Voodoo.Analytics
{
    internal static class AnalyticsUtil
    {
        internal static string ConvertDictionaryToJson(Dictionary<string, object> eventCustomData,
                                                       string dataJson = null,
                                                       string customVariables = null)
        {
            IEnumerable<string> entries = eventCustomData.Select(d => {
                string value;
                if (d.Value == null) {
                    value = "\"\"";
                } else if (d.Value is string) {
                    value = $"\"{d.Value}\"";
                } else if (d.Value is bool) {
                    value = $"{d.Value.ToString().ToLower()}";
                } else {
                    value = $"{d.Value}";
                }

                return $"\"{d.Key}\":{value}";
            });

            string result = "{" + string.Join(",", entries);

            if (ValidateJson(dataJson)) {
                result += ",\"" + AnalyticsConstant.DATA + "\":" + dataJson;
            }

            if (ValidateJson(customVariables)) {
                result += ",\"" + AnalyticsConstant.CUSTOM_VARIABLES + "\":" + customVariables;
            }

            result += "}";

            return result;
        }

        private static bool ValidateJson(string json)
        {
            return !string.IsNullOrEmpty(json) && json.StartsWith("{") && json.EndsWith("}");
        }

        internal static string ConvertDictionaryToCustomVarJson(Dictionary<string, object> eventCustomVariables)
        {
            var result = "";
            var counter = 0;
            foreach (KeyValuePair<string, object> pair in eventCustomVariables) {
                if (!string.IsNullOrEmpty(result)) result += ",";
                result += $"\"c{counter}_key\":\"{pair.Key}\",";
                result += $"\"c{counter}_val\":\"{pair.Value}\"";
                counter++;
            }

            return "{" + result + "}";
        }
    }
}