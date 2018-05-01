using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome
{
    public interface IDeviceManager
    {
        Task<DeviceStateInfo> AddOrUpdate(RequestContext requestContext);
        string GetDeviceId(JObject obj);
        string GetDeviceType(JObject obj);
        void RegisterDeviceTypeProvider(DeviceTypeProvider deviceTypeProvider);
        Task RemoveDevice(Device device);
        IEnumerable<Device> GetDevices();

        IEnumerable<DeviceTypeProvider> DeviceTypeProviders { get; }
    }
}
