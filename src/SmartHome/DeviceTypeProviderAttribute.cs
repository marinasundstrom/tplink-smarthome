using System;

namespace SmartHome
{
    public class DeviceTypeProviderAttribute : Attribute
    {
        public DeviceTypeProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
        }

        public Type ProviderType { get; }
    }
}
