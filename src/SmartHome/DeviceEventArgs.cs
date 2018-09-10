using System;

namespace SmartHome
{
    public class DeviceEventArgs : EventArgs
    {
        public DeviceEventArgs(IDevice device)
        {
            Device = device;
        }

        public IDevice Device { get; set; }
    }
}
