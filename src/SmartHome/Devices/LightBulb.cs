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

            var p = Parameters ?? new LightParameters();

            p.Mode = obj.Value<string>("mode");
            p.Hue = obj.Value<int>("hue");
            p.Saturation = obj.Value<int>("saturation");
            p.ColorTemp = obj.Value<int>("color_temp");
            p.Brightness = obj.Value<int>("brightness");

            Parameters = p;
        }

        public Task TransitionStateAsync(SwitchState onOff, int transitionPeriod = 0)
        {
            var state = new LightBulbState()
            {
                OnOff = onOff
            };
            return TransitionStateAsync(state, transitionPeriod);
        }

        public async Task TransitionStateAsync(LightBulbState state, int transitionPeriod = 0)
        {
            if (IsDimmable != null && !(bool)IsDimmable)
            {
                throw new NotSupportedException("Light bulb is not dimmable.s");
            }
            bool? onOff = null;
            if(state.OnOff != null)
            {
                onOff = state.OnOff == SwitchState.On ? true : false;
            }
            var result = await SendCommand(
                Commands.TransitionLightState(onOff, transitionPeriod, state.Hue, state.Saturation, state.ColorTemp, state.Brightness));

            var obj = ParseSmartBulbTransitionLightStateResponse(result);

            int code = GetErrorCode(obj);

            UpdateBulbState(obj);
        }

        public async Task<SwitchState> GetStateAsync()
        {
            var str = await SendCommand(Commands.GetSysInfo);
            var obj = ParseGetSysInfo(str);

            UpdateInternal(obj);

            return (SwitchState)obj
                .Value<JObject>("light_state")
                .Value<int>("on_off");
        }

        public bool? IsDimmable { get; private set; }

        public bool? IsColor { get; private set; }

        public bool? IsVariableColorTemp { get; private set; }

        public LightParameters Parameters { get; private set; }
    }

    public class LightBulbState
    {
        public SwitchState? OnOff { get; set; }
        public int? Brightness { get; set; }
        public int? Hue { get; set; }
        public int? Saturation { get; set; }
        public int? ColorTemp { get; set; }
    }


    public class LightParameters
    {
        public string Mode { get; internal set; }
        public int Hue { get; internal set; }
        public int Saturation { get; internal set; }
        public int ColorTemp { get; internal set; }
        public int Brightness { get; internal set; }
    }

}
