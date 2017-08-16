using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Devices
{
    public sealed class Plug : Device
    {
        public Plug()
        {
            Type = DeviceType.Plug;
        }

        public async Task<SwitchState> GetRelayStateAsync()
        {
            var message = Commands.GetSysInfo;
            var str = await SendCommand(message);

            var obj = ParserHelpers.ParseGetSysInfo(str);

            UpdateInternal(obj);

            return (SwitchState)obj
                .Value<int>("relay_state");
        }

        public Task SetRelayStateAsync(SwitchState state)
        {
            return SendCommand(
                Commands.SetRelayState(state == SwitchState.On));
        }
    }
}
