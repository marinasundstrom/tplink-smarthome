using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    static class Commands
    {
        private static string getSysInfo;
        private static string getSysInfo2;

        public static string GetSysInfo => getSysInfo ?? (getSysInfo = JsonConvert.SerializeObject(
            new
            {
                system = new
                {
                    get_sysinfo = new
                    {

                    }
                }
            }
        ));

        public static string GetSysInfo2 => getSysInfo2 ?? (getSysInfo2 = JsonConvert.SerializeObject(
            new
            {
                system = new
                {
                    emeter = new
                    {
                        get_realtime = new
                        {

                        }
                    },
                    get_sysinfo = new
                    {

                    }
                }
            }
        ));

        public static string SetRelayState(bool state) => JsonConvert.SerializeObject(new
        {
            system = new
            {
                set_relay_state = new
                {
                    state = state ? 1 : 0
                }
            }
        });

        public static string TransitionLightState(
            bool? onOffState = null,
            int? transitionPeriod = null,
            int? hue = null,
            int? saturation = null,
            int? colorTemp = null,
            int? brightness = null)
        {
            var obj = new JObject();
            var obj2 = new JObject();
            if (onOffState != null)
            {
                obj2["on_off"] = onOffState ?? false ? 1 : 0;
            }
            if (transitionPeriod != null)
            {
                obj2["transition_period"] = transitionPeriod;
            }
            if (hue != null)
            {
                obj2["hue"] = hue;
            }
            if (saturation != null)
            {
                obj2["saturation"] = saturation;
            }
            if (colorTemp != null)
            {
                obj2["color_temp"] = colorTemp;
            }
            if (brightness != null)
            {
                obj2["brightness"] = brightness;
            }
            obj["transition_light_state"] = obj2;
            var obj3 = new JObject();
            obj3["smartlife.iot.smartbulb.lightingservice"] = obj;

            return obj3.ToString(Formatting.None);
        }

        public static string SetDeviceAlias(string alias) => JsonConvert.SerializeObject(new
        {
            system = new
            {
                set_dev_alias = new
                {
                    alias = alias
                }
            }
        });

        public static string SetDeviceId(string id) => JsonConvert.SerializeObject(new
        {
            system = new
            {
                set_device_id = new
                {
                    id = id
                }
            }
        });
    }

}
