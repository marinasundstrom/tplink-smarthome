using System;

namespace SmartHome
{
    public class DeviceDiscoveryEventArgs : EventArgs
    {
        public DeviceDiscoveryEventArgs(Device device)
        {
            Device = device;
        }

        public Device Device { get; set; }
    }

}
