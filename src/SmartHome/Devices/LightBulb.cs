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
        }

        public Task SetTransitionStateAsync(SwitchState state, int transitionPeriod = 0)
        {
            return SendCommand(Commands.TransitionLightState(state == SwitchState.On ? true : false, transitionPeriod));
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

        public bool IsDimmable { get; set; }
        public bool IsColor { get; set; }
        public bool IsVariableColorTemp { get; set; }
    }
}
