using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SmartHome.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    public abstract class Device
    {
        public Device()
        {
            Type = DeviceType.Unknown;
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

        public string IPAddress { get; internal set; }

        public int RSSI { get; private set; }

        public bool IsUpdating { get; private set; }

        internal void UpdateInternal(JObject obj)
        {
            this.Update(obj);
        }

        protected virtual void Update(JObject obj)
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
        }

        internal static string GetMACAddress(JObject obj)
        {
            var result = obj.Value<string>("mac");
            if (result == null)
            {
                result = obj.Value<string>("mic_mac");
            }
            return result;
        }

        public async Task FetchStateAsync()
        {
            var obj = Commands.GetSysInfo;
            var str = await SendCommand(obj);

            Update(JObject.Parse(str));
        }

        protected async Task<string> SendCommand(string request)
        {
            var enc = Encoding.UTF8.GetBytes(request);
            var data = Utils.Encrypt(enc);

            var results = await Send(data);

            var data2 = Utils.Decrypt(results);
            return Encoding.UTF8.GetString(data2);
        }

        protected async Task<byte[]> Send(byte[] data)
        {
            using (var client = new UdpClient(9998))
            {
                var remoteEp = new IPEndPoint(System.Net.IPAddress.Parse(IPAddress), 9999);
                await client.SendAsync(data, data.Length, remoteEp);
                var dataReceived = client.Receive(ref remoteEp);
                return dataReceived.ToArray();
            }
        }

        public static Device FromJson(JObject obj)
        {
            string type = obj.Value<string>("type");
            if (type == null)
            {
                type = obj.Value<string>("mic_type");
            }

            switch (type)
            {
                case "IOT.SMARTPLUGSWITCH":
                    var plug = new Plug();
                    plug.Update(obj);
                    return plug;

                case "IOT.SMARTBULB":
                    var bulb = new LightBulb();
                    bulb.Update(obj);
                    return bulb;
            }

            throw new ArgumentException();
        }

        public Task SetAliasAsync(string alias)
        {
            return SendCommand(Commands.SetDeviceAlias(alias));
        }

        public Task SetIdAsync(string id)
        {
            return SendCommand(Commands.SetDeviceId(id));
        }
    }

}
