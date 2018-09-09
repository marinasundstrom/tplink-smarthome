using System;

namespace SmartHome
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DeviceTypeProviderAttribute : Attribute
    {
        public DeviceTypeProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
        }

        public Type ProviderType { get; }
    }
}
