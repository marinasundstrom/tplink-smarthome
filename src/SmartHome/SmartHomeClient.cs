using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartHome.EncryptionHelpers;

namespace SmartHome
{
    public class SmartHomeClient : IDisposable, ISmartHomeClient
    {
        private Dictionary<string, Device> devices;
        private System.Timers.Timer timer;
        private Socket socket;
        private Task scannerThread;
        private CancellationTokenSource cancellationTokenSource;
        private bool isRunning;

        // Socket
        private IPAddress multicastAddress;
        private int multicastPort;

        public IEnumerable<Device> GetDevices()
        {
            return devices.Select(x => x.Value);
        }

        private IPEndPoint multicastEp;
        private IPEndPoint localEp;

        public SmartHomeClient()
        {
            DiscoveryRate = TimeSpan.FromSeconds(30);
        }

        public static Socket CreateSocket(EndPoint localEp)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.MulticastLoopback = false;

            socket.Bind(localEp);

            return socket;
        }

        private void Initialize()
        {
            multicastAddress = IPAddress.Broadcast;
            multicastPort = 9999;
            multicastEp = new IPEndPoint(multicastAddress, multicastPort);
            localEp = new IPEndPoint(IPAddress.Any, multicastPort);

            socket = CreateSocket(localEp);

            devices = new Dictionary<string, Device>();
            timer = new System.Timers.Timer(DiscoveryRate.TotalMilliseconds);
            timer.Elapsed += (s, e) =>
            {
                Scan();
            };
        }

        public void Start()
        {
            Initialize();
            StartScannerThread();
            Scan();

            isRunning = true;
            timer.Start();
        }

        private void StartScannerThread()
        {
            EndPoint localEp = this.localEp;

            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = cancellationTokenSource.Token;
            scannerThread = Task.Run(() =>
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var response = new byte[8000];
                        var no = socket.ReceiveFrom(response, ref localEp);
                        var str = Encoding.UTF8.GetString(Decrypt(response.Take(no).ToArray()));

                        var obj = ParserHelpers.ParseGetSysInfo(str);

                        if (obj == null) continue;

                        var macAddress = ParserHelpers.GetMACAddress(obj);

                        if (!devices.TryGetValue(macAddress, out var device))
                        {
                            Debug.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));

                            device = Device.FromJson(obj, this);
                            device.IPAddress = ((IPEndPoint)localEp).Address;
                            devices.Add(macAddress, device);

                            DeviceDiscovered?.Invoke(this, new DeviceEventArgs(device));
                        }
                        else
                        {
                            device.UpdateInternal(obj);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }, ct);
        }

        public void Stop()
        {
            timer.Stop();
            socket.Close();
            cancellationTokenSource.Cancel();
            isRunning = false;
        }

        public void Scan()
        {
            var obj = Commands.GetSysInfo;
            var enc = Encoding.UTF8.GetBytes(obj);
            var a = EncryptionHelpers.Encrypt(enc);
            socket.SendTo(a, 0, a.Length, SocketFlags.None, multicastEp);
        }

        public TimeSpan DiscoveryRate { get; set; }

        public bool IsRunning => isRunning;

        public event EventHandler<DeviceEventArgs> DeviceDiscovered;

        public event EventHandler<DeviceEventArgs> DeviceUpdated;

        internal void OnDeviceUpdated(Device device)
        {
            DeviceUpdated?.Invoke(this, new DeviceEventArgs(device));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HS100Client() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
