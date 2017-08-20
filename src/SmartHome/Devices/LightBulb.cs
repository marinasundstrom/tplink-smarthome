using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using static SmartHome.ParserHelpers;

namespace SmartHome.Devices
{
    public class LightBulb : Device
    {
        internal LightBulb()
        {
            Type = DeviceType.LightBulb;
        }

        public LightBulb(IPAddress address): this()
        {
            IPAddress = address;
        }

        protected override void Update(JObject obj)
        {
            base.Update(obj);

            IsDimmable = Convert.ToBoolean(obj.Value<int>("is_dimmable"));
            IsColor = Convert.ToBoolean(obj.Value<int>("is_color"));
            IsVariableColorTemp = Convert.ToBoolean(obj.Value<int>("is_variable_color_temp"));

            UpdateBulbState(obj.Value<JObject>("light_state"));
        }

        private void UpdateBulbState(JObject obj)
        {
            //var state = obj.Value<JObject>("dft_on_state");

            var p = State ?? new LightBulbState();

            p.Mode = obj.Value<string>("mode");
            p.PowerState = (SwitchState)obj.Value<int>("on_off");
            p.Hue = obj.Value<int>("hue");
            p.Saturation = obj.Value<int>("saturation");
            p.ColorTemp = obj.Value<int>("color_temp");
            p.Brightness = obj.Value<int>("brightness");

            State = p;
        }

        public Task TransitionStateAsync(SwitchState powerState, int transitionPeriod = 0)
        {
            var state = new RequestedState()
            {
                PowerState = powerState
            };
            return TransitionStateAsync(state, transitionPeriod);
        }

        public async Task TransitionStateAsync(RequestedState state, int transitionPeriod = 0)
        {
            if (IsDimmable != null && !(bool)IsDimmable)
            {
                throw new NotSupportedException("Light bulb is not dimmable.s");
            }
            bool? powerState = null;
            if(state.PowerState != null)
            {
                powerState = state.PowerState == SwitchState.On ? true : false;
            }
            var result = await SendCommand(
                Commands.TransitionLightState(powerState, transitionPeriod, state.Hue, state.Saturation, state.ColorTemp, state.Brightness));

            var obj = ParseSmartBulbTransitionLightStateResponse(result);

            int code = GetErrorCode(obj);

            UpdateBulbState(obj);
        }

        public async Task<LightBulbState> GetStateAsync()
        {
            var str = await SendCommand(Commands.GetSysInfo);
            var obj = ParseGetSysInfo(str);

            UpdateInternal(obj);

            return State;
        }

        public bool? IsDimmable { get; private set; }

        public bool? IsColor { get; private set; }

        public bool? IsVariableColorTemp { get; private set; }

        public LightBulbState State { get; private set; }
    }

    public struct RequestedState
    {
        public SwitchState? PowerState { get; set; }
        public int? Brightness { get; set; }
        public int? Hue { get; set; }
        public int? Saturation { get; set; }
        public int? ColorTemp { get; set; }
    }


    public class LightBulbState
    {
        public string Mode { get; internal set; }
        public SwitchState PowerState { get; internal set; }
        public int Hue { get; internal set; }
        public int Saturation { get; internal set; }
        public int ColorTemp { get; internal set; }
        public int Brightness { get; internal set; }
    }

}
