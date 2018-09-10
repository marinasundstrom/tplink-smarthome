using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartHome.Devices;
using static SmartHome.ParserHelpers;

namespace SmartHome
{
#pragma warning disable CA1012 // Abstract types should not have constructors
    public abstract class Device : IDevice
#pragma warning restore CA1012 // Abstract types should not have constructors
    {
        private readonly SmartHomeClient _client;
        private DeviceTypeProvider _deviceTypeProvider;

#pragma warning disable RCS1160 // Abstract type should not have public constructors.
        public Device()
#pragma warning restore RCS1160 // Abstract type should not have public constructors.
        {
            Type = DeviceType.Unknown;
        }

        internal Device(SmartHomeClient client) : this()
        {
            _client = client;
        }

        public string Alias { get; internal set; }

        public string Description { get; internal set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType Type { get; internal set; }

        public string DeviceTypeId { get; internal set; }

        public string Model { get; internal set; }

        public string HardwareVersion { get; internal set; }

        public string SoftwareVersion { get; internal set; }

        public string MAC { get; internal set; }

        public string DeviceId { get; internal set; }

        public string HardwareId { get; internal set; }

        public string FirmwareId { get; internal set; }

        public string OEMId { get; internal set; }

        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPAddress { get; internal set; }

        public int RSSI { get; internal set; }

        public bool IsUpdating { get; internal set; }

        public async Task SetAliasAsync(string alias)
        {
            await SendCommand(Commands.SetDeviceAlias(alias)).ConfigureAwait(false);
            Alias = alias;
        }

        public async Task SetDeviceIdAsync(string id)
        {
            await SendCommand(Commands.SetDeviceId(id)).ConfigureAwait(false);
            DeviceId = id;
        }

        public Task<string> SendCommand(string command)
        {
            return CommandHelper.SendCommand(
                IPAddress,
                command);
        }

        public async Task FetchAsync()
        {
            _deviceTypeProvider = _deviceTypeProvider ?? this.GetDeviceTypeProvider();

            string str = await SendCommand(Commands.GetSysInfo).ConfigureAwait(false);
            Newtonsoft.Json.Linq.JObject obj = ParseGetSysInfo(str);

            var requestContext = new RequestContext(obj, IPAddress);
            await _deviceTypeProvider.UpdateDevice(this, requestContext).ConfigureAwait(false);
        }
    }
}
