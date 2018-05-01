using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SmartHome.Devices;
using System;
using static SmartHome.ParserHelpers;
using System.Threading.Tasks;
using System.Net;

namespace SmartHome
{
    public abstract class Device
    {
        private SmartHomeClient client;
        private DeviceTypeProvider deviceTypeProvider;

        public Device()
        {
            Type = DeviceType.Unknown;
        }

        internal Device(SmartHomeClient client) : this()
        {
            this.client = client;
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
            await SendCommand(Commands.SetDeviceAlias(alias));
            Alias = alias;
        }

        public async Task SetDeviceIdAsync(string id)
        {
            await SendCommand(Commands.SetDeviceId(id));
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
            deviceTypeProvider = deviceTypeProvider ?? this.GetDeviceTypeProvider();

            var str = await SendCommand(Commands.GetSysInfo);
            var obj = ParseGetSysInfo(str);

            var requestContext = new RequestContext(obj, IPAddress);
            await deviceTypeProvider.UpdateDevice(this, requestContext);
        }
    }

}
