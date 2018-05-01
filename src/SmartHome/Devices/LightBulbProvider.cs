using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome.Devices
{
    public class LightBulbProvider : DeviceTypeProvider
    {
        private const string DEVICE_TYPE = "IOT.SMARTBULB";
        private static LightBulbProvider instance;

        public override string DeviceType => DEVICE_TYPE;

        public override async Task<Device> CreateDevice(RequestContext requestContext)
        {
            var device = new LightBulb();

            SetCommonDeviceProperties(device, requestContext);
            SetBulbProperties(device, requestContext.Data);

            return device;
        }

        public override async Task<bool> UpdateDevice(Device device, RequestContext requestContext)
        {
            SetCommonDeviceProperties(device, requestContext);
            SetBulbProperties(device as LightBulb, requestContext.Data);

            UpdateBulbState(device as LightBulb, requestContext.Data.Value<JObject>("light_state"));

            return true;
        }

        private bool SetBulbProperties(LightBulb device, JObject obj)
        {
            device.IsDimmable = Convert.ToBoolean(obj.Value<int>("is_dimmable"));
            device.IsColor = Convert.ToBoolean(obj.Value<int>("is_color"));
            device.IsVariableColorTemp = Convert.ToBoolean(obj.Value<int>("is_variable_color_temp"));

            return true;
        }

        private void UpdateBulbState(LightBulb device, JObject obj)
        {
            var p = device.State ?? new LightBulbState();

            p.Mode = obj.Value<string>("mode");
            p.PowerState = (SwitchState)obj.Value<int>("on_off");
            p.Hue = obj.Value<int>("hue");
            p.Saturation = obj.Value<int>("saturation");
            p.ColorTemp = obj.Value<int>("color_temp");
            p.Brightness = obj.Value<int>("brightness");

            device.State = p;
        }

        public static DeviceTypeProvider Instance => instance ?? (instance = new LightBulbProvider());
    }
}
