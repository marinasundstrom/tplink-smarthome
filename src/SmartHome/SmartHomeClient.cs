using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartHome.Devices;
using static SmartHome.EncryptionHelpers;

namespace SmartHome
{
    public class SmartHomeClient : ISmartHomeClient
    {
        private IDeviceManager _deviceManager;
        private System.Timers.Timer _timer;
        private Socket _socket;
        private Task _scannerThread;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

        // Socket
        private IPAddress _multicastAddress;
        private int _multicastPort;

        private IPEndPoint _multicastEp;
        private IPEndPoint _localEp;

        public SmartHomeClient(DeviceType[] deviceTypeFilter = null)
        {
            DiscoveryRate = TimeSpan.FromSeconds(30);
            SetDeviceTypeFilter(deviceTypeFilter);

            InitializeDeviceManager();
        }

        private void InitializeDeviceManager()
        {
            _deviceManager = new DeviceManager();
            _deviceManager.RegisterDeviceTypeProvider(LightBulbProvider.Instance);
            _deviceManager.RegisterDeviceTypeProvider(PlugProvider.Instance);
        }

        public static Socket CreateSocket(EndPoint localEp)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true,
                MulticastLoopback = false
            };

            socket.Bind(localEp);

            return socket;
        }

        private void Initialize()
        {
            _multicastAddress = IPAddress.Broadcast;
            _multicastPort = 9999;
            _multicastEp = new IPEndPoint(_multicastAddress, _multicastPort);
            _localEp = new IPEndPoint(IPAddress.Any, _multicastPort);

            _socket = CreateSocket(_localEp);

            _timer = new System.Timers.Timer(DiscoveryRate.TotalMilliseconds);
            _timer.Elapsed += (s, e) => Scan();
        }

        public void Start()
        {
            Initialize();
            StartScannerThread();
            Scan();

            _isRunning = true;
            _timer.Start();
        }

        private void StartScannerThread()
        {
            EndPoint localEp = _localEp;

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = _cancellationTokenSource.Token;
            _scannerThread = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        byte[] response = new byte[8000];
                        int no = _socket.ReceiveFrom(response, ref localEp);
                        string str = Encoding.UTF8.GetString(Decrypt(response.Take(no).ToArray()));

                        JObject obj = ParserHelpers.ParseGetSysInfo(str);

                        if (obj == null)
                        {
                            continue;
                        }

                        DeviceType deviceType = ParserHelpers.GetDeviceType(obj);

                        if (GetDeviceTypeFilter()?.Contains(deviceType) == false)
                        {
                            Debug.WriteLine("Excluded device");
                            continue;
                        }

                        var requestContext = new RequestContext(obj, (localEp as IPEndPoint)?.Address);
                        DeviceStateInfo state = await _deviceManager.AddOrUpdate(requestContext).ConfigureAwait(false);

                        switch (state.State)
                        {
                            case DeviceState.Added:
                                DeviceDiscovered?.Invoke(this, new DeviceEventArgs(state.Device));
                                break;

                            case DeviceState.Updated:
                                DeviceUpdated?.Invoke(this, new DeviceEventArgs(state.Device));
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }, ct);
        }

        public IEnumerable<IDevice> GetDevices() => _deviceManager.GetDevices();

        public void Stop()
        {
            _timer.Stop();
            _socket.Close();
            _cancellationTokenSource.Cancel();
            _isRunning = false;
        }

        public void Scan()
        {
            string obj = Commands.GetSysInfo;
            byte[] enc = Encoding.UTF8.GetBytes(obj);
            byte[] a = Encrypt(enc);
            _socket.SendTo(a, 0, a.Length, SocketFlags.None, _multicastEp);
        }

        public TimeSpan DiscoveryRate { get; set; }

        public DeviceType[] GetDeviceTypeFilter()
        {
            return _deviceTypeFilter;
        }

        public void SetDeviceTypeFilter(DeviceType[] value)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot set filter when running.");
            }
            _deviceTypeFilter = value;
        }

        public bool IsRunning => _isRunning;

        public event EventHandler<DeviceEventArgs> DeviceDiscovered;

        public event EventHandler<DeviceEventArgs> DeviceUpdated;

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls
        private DeviceType[] _deviceTypeFilter;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                _disposedValue = true;
            }
        }

        ~SmartHomeClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
