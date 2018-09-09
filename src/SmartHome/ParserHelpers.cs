using Newtonsoft.Json.Linq;
using SmartHome.Devices;

namespace SmartHome
{
    internal static class ParserHelpers
    {
        public static string GetMACAddress(JObject obj)
        {
            string result = obj.Value<string>("mac");
            if (result == null)
            {
                result = obj.Value<string>("mic_mac");
                result = FormatMACAddress(result);
            }
            return result;
        }

        private static string FormatMACAddress(string result)
        {
            string r = string.Empty;
            for (int i = 0; i < result.Length; i += 2)
            {
                string pair = $"{result[i]}{result[i + 1]}";
                if (r?.Length == 0)
                {
                    r = pair;
                }
                else
                {
                    r += $":{pair}";
                }
            }
            return r;
        }

        public static JObject ParseGetSysInfo(string input)
        {
            JObject obj = JObject.Parse(input)?
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

        public static JObject ParseSmartBulbTransitionLightStateResponse(string input)
        {
            JObject obj = JObject.Parse(input)?
                                .Value<JObject>("smartlife.iot.smartbulb.lightingservice")?
                                .Value<JObject>("transition_light_state");

            if (obj?.IsNullOrEmpty() ?? false)
            {
                return null;
            }
            else
            {
                return obj;
            }
        }

        public static int GetErrorCode(JObject obj)
        {
            int code = 0;
            if (obj.TryGetValue("error_code", out JToken token))
            {
                code = token.Value<int>();
            }
            return code;
        }

        public static string GetDeviceTypeString(JObject obj)
        {
            string type = obj.Value<string>("type") ?? obj.Value<string>("mic_type");
            return type;
        }

        public static DeviceType GetDeviceType(JObject obj)
        {
            string type = GetDeviceTypeString(obj);

            switch (type)
            {
                case "IOT.SMARTPLUGSWITCH":
                    return DeviceType.Plug;

                case "IOT.SMARTBULB":
                    return DeviceType.LightBulb;
            }

            return DeviceType.Unknown;
        }
    }
}
