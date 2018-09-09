using System;
using System.Threading.Tasks;

namespace SmartHome
{
    public abstract class DeviceTypeProvider
    {
        public abstract string DeviceType { get; }

        public abstract Task<Device> CreateDevice(RequestContext requestContext);

        public abstract Task<bool> UpdateDevice(Device device, RequestContext requestContext);

        protected virtual bool SetCommonDeviceProperties(Device device, RequestContext requestContext)
        {
            Newtonsoft.Json.Linq.JObject obj = requestContext.Data;

            device.Alias = obj.Value<string>("alias");
            device.Description = obj.Value<string>("description");
            device.Model = obj.Value<string>("model");
            device.DeviceTypeId = obj.Value<string>("type") ?? obj.Value<string>("mic_type");
            device.HardwareVersion = obj.Value<string>("sw_ver");
            device.SoftwareVersion = obj.Value<string>("hw_ver");
            device.MAC = ParserHelpers.GetMACAddress(obj);
            device.DeviceId = obj.Value<string>("deviceId");
            device.HardwareId = obj.Value<string>("hwId");
            device.FirmwareId = obj.Value<string>("fwId");
            device.OEMId = obj.Value<string>("oemId");

            device.RSSI = obj.Value<int>("rssi");

            device.IsUpdating = Convert.ToBoolean(obj.Value<int>("updating"));

            if (device.IPAddress == null)
            {
                device.IPAddress = requestContext.Address;
            }

            return true;
        }
    }
}
