using System;

namespace SmartHome
{
    public class DeviceEventArgs : EventArgs
    {
        public DeviceEventArgs(Device device)
        {
            Device = device;
        }

        public Device Device { get; set; }
    }
}
