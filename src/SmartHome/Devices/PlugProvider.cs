using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome.Devices
{
    public class PlugProvider : DeviceTypeProvider
    {
        private const string DEVICE_TYPE = "IOT.SMARTPLUGSWITCH";
        private static PlugProvider s_instance;

        public override string DeviceType => DEVICE_TYPE;

        public override Task<Device> CreateDevice(RequestContext requestContext)
        {
            var device = new Plug();

            SetCommonDeviceProperties(device, requestContext);
            UpdateRelatState(device, requestContext.Data);

            return Task.FromResult<Device>(device);
        }

        public override Task<bool> UpdateDevice(Device device, RequestContext requestContext)
        {
            SetCommonDeviceProperties(device, requestContext);
            UpdateRelatState(device, requestContext.Data);

            return Task.FromResult(true);
        }

        private static bool UpdateRelatState(Device device, JObject obj)
        {
            ((Plug)device).RelayState = (SwitchState)obj
                .Value<int>("relay_state");

            return true;
        }

        public static DeviceTypeProvider Instance => s_instance ?? (s_instance = new PlugProvider());
    }
}
