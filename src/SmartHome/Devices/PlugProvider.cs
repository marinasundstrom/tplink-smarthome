using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome.Devices
{
    public class PlugProvider : DeviceTypeProvider
    {
        private const string DEVICE_TYPE = "IOT.SMARTPLUGSWITCH";
        private static PlugProvider instance;

        public override string DeviceType => DEVICE_TYPE;

        public override async Task<Device> CreateDevice(RequestContext requestContext)
        {
            var device = new Plug();

            SetCommonDeviceProperties(device, requestContext);
            UpdateRelatState(device, requestContext.Data);

            return device;
        }

        public override async Task<bool> UpdateDevice(Device device, RequestContext requestContext)
        {
            SetCommonDeviceProperties(device, requestContext);
            UpdateRelatState(device, requestContext.Data);

            return true;
        }

        private static bool UpdateRelatState(Device device, JObject obj)
        {
            (device as Plug).RelayState = (SwitchState)obj
                .Value<int>("relay_state");

            return true;
        }

        public static DeviceTypeProvider Instance => instance ?? (instance = new PlugProvider());
    }
}
