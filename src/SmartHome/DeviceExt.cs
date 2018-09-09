using System.Linq;

namespace SmartHome
{
    public static class DeviceExt
    {
        public static DeviceTypeProvider GetDeviceTypeProvider(this Device device)
        {
            DeviceTypeProviderAttribute deviceTypeProviderAttribute = device.GetType()
                .GetCustomAttributes(false)
                .OfType<DeviceTypeProviderAttribute>()
                .Single();

            return (DeviceTypeProvider)deviceTypeProviderAttribute.ProviderType
                .GetProperty("Instance", System.Reflection.BindingFlags.Static)
                .GetValue(null);
        }
    }
}
