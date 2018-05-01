using System.Linq;
using System.Threading.Tasks;

namespace SmartHome
{
    public static class DeviceExt
    { 
        public static DeviceTypeProvider GetDeviceTypeProvider(this Device device)
        {
            var deviceTypeProviderAttribute = device.GetType()
                .GetCustomAttributes(false)
                .OfType<DeviceTypeProviderAttribute>()
                .Single();

            return (DeviceTypeProvider)deviceTypeProviderAttribute.ProviderType
                .GetProperty("Instance", System.Reflection.BindingFlags.Static)
                .GetValue(null);
        }
    }

}
