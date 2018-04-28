using System;
using System.Collections.Generic;

namespace SmartHome
{
    public interface ISmartHomeClient
    {
        TimeSpan DiscoveryRate { get; set; }
        bool IsRunning { get; }

        event EventHandler<DeviceEventArgs> DeviceDiscovered;
        event EventHandler<DeviceEventArgs> DeviceUpdated;

        IEnumerable<Device> GetDevices();
        void Scan();
        void Start();
        void Stop();
    }
}
