using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Devices
{
    public class LightBulb : Device
    {
        public LightBulb()
        {
            Type = DeviceType.LightBulb;
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
            var state = obj.Value<JObject>("dft_on_state");

            Parameters = new LightParameters()
            {
                Mode = state.Value<string>("mode"),
                Hue = state.Value<int>("hue"),
                Saturation = state.Value<int>("saturation"),
                ColorTemp = state.Value<int>("color_temp"),
                Brightness = state.Value<int>("brightness")
            };
        }

        public Task TransitionStateAsync(SwitchState state, int transitionPeriod = 0, int hue = 0, int saturation = 0, int colorTemp = 2700, int brightness = 0)
        {
            if(!IsDimmable)
            {
                throw new NotSupportedException("Light bulb is not dimmable.s");
            }
            return SendCommand(
                Commands.TransitionLightState(state == SwitchState.On ? true : false,
                transitionPeriod, hue, saturation, colorTemp, brightness));
        }

        public async Task<SwitchState> GetStateAsync()
        {
            var message = Commands.GetSysInfo;
            var str = await SendCommand(message);

            var obj = ParserHelpers.ParseGetSysInfo(str);

            UpdateInternal(obj);

            return (SwitchState)obj
                .Value<JObject>("light_state")
                .Value<int>("on_off");
        }

        public bool IsDimmable { get; private set; }

        public bool IsColor { get; private set; }

        public bool IsVariableColorTemp { get; private set; }

        public LightParameters Parameters { get; private set; }
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
