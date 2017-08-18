using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.CommandHelper;
using static SmartHome.ParserHelpers;

namespace SmartHome.Devices
{
    public sealed class Plug : Device
    {
        internal Plug()
        {
            Type = DeviceType.Plug;
        }

        public Plug(IPAddress address) : this()
        {
            IPAddress = address;
        }

        public async Task<SwitchState> GetRelayStateAsync()
        {
            var message = Commands.GetSysInfo;
            var str = await SendCommand(message);

            var obj = ParseGetSysInfo(str);

            UpdateInternal(obj);

            return (SwitchState)obj
                .Value<int>("relay_state");
        }

        public async Task SetRelayStateAsync(SwitchState state)
        {
            var result = await SendCommand(
                Commands.SetRelayState(state == SwitchState.On));

            var obj = ParseSmartBulbTransitionLightStateResponse(result);

            //UpdateBulbState(obj);
        }
    }
}
