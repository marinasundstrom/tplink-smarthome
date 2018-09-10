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

            System.Reflection.PropertyInfo prop = deviceTypeProviderAttribute.ProviderType
                .GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return (DeviceTypeProvider)prop.GetValue(null);
        }
    }
}
