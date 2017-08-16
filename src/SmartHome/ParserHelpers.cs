using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    static class ParserHelpers
    {
        public static string GetMACAddress(JObject obj)
        {
            var result = obj.Value<string>("mac");
            if (result == null)
            {
                result = obj.Value<string>("mic_mac");
                result = FormatMACAddress(result);
            }
            return result;
        }

        private static string FormatMACAddress(string result)
        {
            var r = string.Empty;
            for (int i = 0; i < result.Length; i += 2)
            {
                string pair = $"{result[i]}{result[i + 1]}";
                if (r == string.Empty)
                {
                    r = pair;
                }
                else
                {
                    r += $":{pair}";
                }
            }
            result = r;
            return result;
        }

        public static JObject ParseGetSysInfo(string input)
        {
            var obj = JObject.Parse(input)?
                                .Value<JObject>("system")?
                                .Value<JObject>("get_sysinfo");

            if (obj?.IsNullOrEmpty() ?? false)
            {
                return null;
            }
            else
            {
                return obj;
            }
        }
    }
}
