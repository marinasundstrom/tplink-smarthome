using System;
using System.Collections.Generic;

namespace SmartHome
{
    public interface ISmartHomeClient : IDisposable
    {
        TimeSpan DiscoveryRate { get; set; }
        bool IsRunning { get; }

        event EventHandler<DeviceEventArgs> DeviceDiscovered;
        event EventHandler<DeviceEventArgs> DeviceUpdated;

        IEnumerable<IDevice> GetDevices();
        void Scan();
        void Start();
        void Stop();
    }
}
