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
        private SmartHomeClient _client;

        public Device()
        {
            Type = DeviceType.Unknown;
        }

        internal Device(SmartHomeClient client) : this()
        {
            _client = client;
        }

        public string Alias { get; protected set; }

        public string Description { get; protected set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType Type { get; protected set; }

        public string DeviceTypeId { get; protected set; }

        public string Model { get; protected set; }

        public string HardwareVersion { get; protected set; }

        public string SoftwareVersion { get; protected set; }

        public string MAC { get; protected set; }

        public string DeviceId { get; protected set; }

        public string HardwareId { get; protected set; }

        public string FirmwareId { get; protected set; }

        public string OEMId { get; protected set; }

        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPAddress { get; internal set; }

        public int RSSI { get; private set; }

        public bool IsUpdating { get; private set; }

        internal void UpdateInternal(JObject obj)
        {
            if (Update(obj))
            {
                OnUpdated();
            }
        }

        protected void OnUpdated()
        {
            _client?.OnDeviceUpdated(this);
        }

        protected virtual bool Update(JObject obj)
        {
            Alias = obj.Value<string>("alias");
            Description = obj.Value<string>("description");
            Model = obj.Value<string>("model");
            DeviceTypeId = obj.Value<string>("type");
            if (DeviceTypeId == null)
            {
                DeviceTypeId = obj.Value<string>("mic_type");
            }
            HardwareVersion = obj.Value<string>("sw_ver");
            SoftwareVersion = obj.Value<string>("hw_ver");
            MAC = GetMACAddress(obj);
            DeviceId = obj.Value<string>("deviceId");
            HardwareId = obj.Value<string>("hwId");
            FirmwareId = obj.Value<string>("fwId");
            OEMId = obj.Value<string>("oemId");

            RSSI = obj.Value<int>("rssi");

            IsUpdating = Convert.ToBoolean(obj.Value<int>("updating"));

            return true;
        }

        public async Task FetchAsync()
        {
            var str = await SendCommand(Commands.GetSysInfo);
            var obj = ParseGetSysInfo(str);

            UpdateInternal(obj);
        }

        public static Device FromJson(JObject obj)
        {
            return FromJson(obj);
        }

        internal static Device FromJson(JObject obj, SmartHomeClient client)
        {
            string type = obj.Value<string>("type");
            if (type == null)
            {
                type = obj.Value<string>("mic_type");
            }

            switch (type)
            {
                case "IOT.SMARTPLUGSWITCH":
                    var plug = new Plug(client);
                    plug.Update(obj);
                    return plug;

                case "IOT.SMARTBULB":
                    var bulb = new LightBulb(client);
                    bulb.Update(obj);
                    return bulb;
            }

            throw new ArgumentException();
        }

        public async Task SetAliasAsync(string alias)
        {
            await SendCommand(Commands.SetDeviceAlias(alias));
            Alias = alias;
            OnUpdated();
        }

        public async Task SetDeviceIdAsync(string id)
        {
            await SendCommand(Commands.SetDeviceId(id));
            DeviceId = id;
            OnUpdated();
        }

        protected Task<string> SendCommand(string command)
        {
            return CommandHelper.SendCommand(
                IPAddress,
                command);
        }
    }

}
