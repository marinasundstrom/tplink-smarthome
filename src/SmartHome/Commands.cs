using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    static class Commands
    {
        public static readonly string GetSysInfo = "{\"system\":{\"get_sysinfo\":{}}}";

        public static readonly string GetSysInfo2 = "{\"emeter\":{\"get_realtime\":{}}, \"system\":{\"get_sysinfo\":{}}}";

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

        public static string TransitionLightState(bool state, int transitionPeriod) => "{ \"smartlife.iot.smartbulb.lightingservice\": " + JsonConvert.SerializeObject(new
        {
            transition_light_state = new
            {
                on_off = state ? 1 : 0,
                transition_period = transitionPeriod
            }
        }) + " }";

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
