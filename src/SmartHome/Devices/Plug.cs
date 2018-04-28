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

        internal Plug(SmartHomeClient client) : base(client)
        {
            Type = DeviceType.Plug;
        }

        public Plug(IPAddress address) : this()
        {
            IPAddress = address;
        }

        public SwitchState RelayState { get; private set; }

        protected override bool Update(JObject obj)
        {
            base.Update(obj);

            RelayState = (SwitchState)obj
                .Value<int>("relay_state");

            return true;
        }

        public async Task SetRelayStateAsync(SwitchState state)
        {
            await SendCommand(
                Commands.SetRelayState(state == SwitchState.On));
            RelayState = state;
            OnUpdated();
        }
    }
}
