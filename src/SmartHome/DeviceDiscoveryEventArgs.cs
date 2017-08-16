using System;
using System.Collections.Generic;
using System.Text;

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
