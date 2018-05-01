using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome
{

    public class DeviceManager : IDeviceManager
    {
        private List<DeviceTypeProvider> deviceTypeProviders = new List<DeviceTypeProvider>();
        private List<Device> devices = new List<Device>();

        public async Task<DeviceStateInfo> AddOrUpdate(RequestContext requestContext)
        {
            var deviceId = GetDeviceId(requestContext.Data);
            var deviceType = GetDeviceType(requestContext.Data);
            var deviceTypeProvider = deviceTypeProviders.Find(dtp => dtp.DeviceType == deviceType);
            if (deviceTypeProvider != null)
            {
                DeviceStateInfo state;
                var device = devices.Find(d => d.DeviceId == deviceId);
                if (device != null)
                {
                    state = await UpdateDevice(requestContext, deviceTypeProvider, device);
                }
                else
                {
                    state = await CreateAndAddDevice(requestContext, deviceTypeProvider);
                    devices.Add(state.Device);
                }
                return state;
            }
            else
            {
                throw new Exception("DeviceTypeProvider not found.");
            }
        }

        private static async Task<DeviceStateInfo> UpdateDevice(RequestContext requestContext, DeviceTypeProvider deviceTypeProvider, Device device)
        {
            var hasChanged = await deviceTypeProvider.UpdateDevice(device, requestContext);
            return new DeviceStateInfo(device, hasChanged ? DeviceState.Updated : DeviceState.Unchanged);
        }

        private static async Task<DeviceStateInfo> CreateAndAddDevice(RequestContext requestContext, DeviceTypeProvider deviceTypeProvider)
        {
            Device device = null;

            device = await deviceTypeProvider.CreateDevice(requestContext);
            return new DeviceStateInfo(device, DeviceState.Added);
        }

        public async Task RemoveDevice(Device device)
        {
            devices.Remove(device);
        }

        public void RegisterDeviceTypeProvider(DeviceTypeProvider deviceTypeProvider)
        {
            deviceTypeProviders.Add(deviceTypeProvider);
        }

        public string GetDeviceId(JObject obj)
        {
            return obj.Value<string>("deviceId");
        }

        public string GetDeviceType(JObject obj)
        {
            string type = obj.Value<string>("type");
            if (type == null)
            {
                type = obj.Value<string>("mic_type");
            }
            return type;
        }

        public IEnumerable<Device> GetDevices() => devices.ToArray();

        public IEnumerable<DeviceTypeProvider> DeviceTypeProviders => deviceTypeProviders;
    }
}
