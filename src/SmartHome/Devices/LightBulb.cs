using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static SmartHome.ParserHelpers;

namespace SmartHome.Devices
{
    [DeviceTypeProvider(typeof(LightBulbProvider))]
    public class LightBulb : Device, ILightBulb
    {
        internal LightBulb()
        {
            Type = DeviceType.LightBulb;
        }

        internal LightBulb(SmartHomeClient client) : base(client)
        {
            Type = DeviceType.LightBulb;
        }

        public LightBulb(IPAddress address) : this()
        {
            IPAddress = address;
        }

        private void UpdateBulbState(JObject obj)
        {
            LightBulbState p = State ?? new LightBulbState();

            p.Mode = obj.Value<string>("mode");
            p.PowerState = (SwitchState)obj.Value<int>("on_off");
            p.Hue = obj.Value<int>("hue");
            p.Saturation = obj.Value<int>("saturation");
            p.ColorTemp = obj.Value<int>("color_temp");
            p.Brightness = obj.Value<int>("brightness");

            State = p;
        }

        public async Task TransitionStateAsync(SwitchState powerState, int transitionPeriod = 0)
        {
            var state = new RequestedBulbState()
            {
                PowerState = powerState
            };
            await TransitionStateAsync(state, transitionPeriod)
                .ConfigureAwait(false);
        }

        public async Task TransitionStateAsync(RequestedBulbState state, int transitionPeriod = 0)
        {
            if (IsDimmable != null && !(bool)IsDimmable)
            {
                throw new NotSupportedException("Light bulb is not dimmable.s");
            }
            bool? powerState = null;
            if (state.PowerState != null)
            {
                powerState = state.PowerState == SwitchState.On;
            }
            string result = await SendCommand(
                Commands
                    .TransitionLightState(powerState, transitionPeriod, state.Hue, state.Saturation, state.ColorTemp, state.Brightness))
                    .ConfigureAwait(false);

            JObject obj = ParseSmartBulbTransitionLightStateResponse(result);

            int code = GetErrorCode(obj);

            UpdateBulbState(obj);
        }

        public bool? IsDimmable { get; internal set; }

        public bool? IsColor { get; internal set; }

        public bool? IsVariableColorTemp { get; internal set; }

        public LightBulbState State { get; internal set; }
    }
}
