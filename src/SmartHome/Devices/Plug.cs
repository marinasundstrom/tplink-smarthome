using System.Net;
using System.Threading.Tasks;

namespace SmartHome.Devices
{
    [DeviceTypeProvider(typeof(PlugProvider))]
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

        public SwitchState RelayState { get; internal set; }

        public async Task SetRelayStateAsync(SwitchState state)
        {
            await SendCommand(
                Commands.SetRelayState(state == SwitchState.On))
                .ConfigureAwait(false);
            RelayState = state;
        }
    }
}
