using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome
{
    public class DeviceManager : IDeviceManager
    {
        private readonly List<DeviceTypeProvider> _deviceTypeProviders = new List<DeviceTypeProvider>();
        private readonly List<Device> _devices = new List<Device>();

        public async Task<DeviceStateInfo> AddOrUpdate(RequestContext requestContext)
        {
            string deviceId = GetDeviceId(requestContext.Data);
            string deviceType = GetDeviceType(requestContext.Data);
            DeviceTypeProvider deviceTypeProvider = _deviceTypeProviders.Find(dtp => dtp.DeviceType == deviceType);
            if (deviceTypeProvider != null)
            {
                DeviceStateInfo state;
                Device device = _devices.Find(d => d.DeviceId == deviceId);
                if (device != null)
                {
                    state = await UpdateDevice(requestContext, deviceTypeProvider, device).ConfigureAwait(false);
                }
                else
                {
                    state = await CreateAndAddDevice(requestContext, deviceTypeProvider).ConfigureAwait(false);
                    _devices.Add(state.Device);
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
            bool hasChanged = await deviceTypeProvider.UpdateDevice(device, requestContext).ConfigureAwait(false);
            return new DeviceStateInfo(device, hasChanged ? DeviceState.Updated : DeviceState.Unchanged);
        }

        private static async Task<DeviceStateInfo> CreateAndAddDevice(RequestContext requestContext, DeviceTypeProvider deviceTypeProvider)
        {
            Device device = await deviceTypeProvider
                .CreateDevice(requestContext)
                .ConfigureAwait(false);
            return new DeviceStateInfo(device, DeviceState.Added);
        }

        public Task RemoveDevice(Device device)
        {
            return Task.Run(() => _devices.Remove(device));
        }

        public void RegisterDeviceTypeProvider(DeviceTypeProvider deviceTypeProvider)
        {
            _deviceTypeProviders.Add(deviceTypeProvider);
        }

        public string GetDeviceId(JObject obj)
        {
            return obj.Value<string>("deviceId");
        }

        public string GetDeviceType(JObject obj)
        {
            string type = obj.Value<string>("type") ?? obj.Value<string>("mic_type");
            return type;
        }

        public IEnumerable<Device> GetDevices() => _devices.ToArray();

        public IEnumerable<DeviceTypeProvider> DeviceTypeProviders => _deviceTypeProviders;
    }
}
