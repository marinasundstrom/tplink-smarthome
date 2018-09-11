using System;
using System.Net;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using SHClient.Services;
using SmartHome.Devices;

namespace SHClient.Commands
{
    [Command("bulb", Description = "Control a bulb."), HelpOption]
    public class BulbCommand
    {
        private readonly ILogger<BulbCommand> _logger;
        private readonly IJsonService _jsonService;

        public BulbCommand(ILogger<BulbCommand> logger, IJsonService jsonService)
        {
            _logger = logger;
            _jsonService = jsonService;
        }

        [Argument(0, Description = "The address of the plug to control.")]
        public string TargetDevice { get; }

        [Option("--state|-s", Description = "Sets the on/off state of the lightbulb.")]
        public string DesiredState { get; }

        [Option("--brightness|-b", Description = "Sets the on/off state of the lightbulb.")]
        public int? Brightness { get; }

        [Option("--json|-j", Description = "Output device info in JSON.")]
        public bool ShowJson { get; }

        public async Task<int> OnExecuteAsync(IConsole console)
        {
            var address = IPAddress.Parse(TargetDevice);

            if (ShowJson)
            {
                var bulb = new LightBulb(address);
                await bulb.FetchAsync().ConfigureAwait(false);
                console.WriteLine(_jsonService.Serialize(bulb));
            }
            else
            {
                var desiredState = new RequestedBulbState();

                SwitchState desiredSwitchState = SwitchState.Off;

                bool shouldToggleState = false;

                if (string.Equals(DesiredState, "toggle", StringComparison.InvariantCultureIgnoreCase))
                {
                    shouldToggleState = true;
                }
                else if (Enum.TryParse(DesiredState, true, out desiredSwitchState))
                {
                    desiredState.PowerState = desiredSwitchState;
                }
                else
                {
                    throw new Exception("Invalid value for parameter \"state\".");
                }
                if (Brightness != null)
                {
                    desiredState.Brightness = Brightness;
                }

                var bulb = new LightBulb(address);
                if (shouldToggleState)
                {
                    await bulb.FetchAsync().ConfigureAwait(false);
                    desiredState.PowerState = bulb.State.PowerState == SwitchState.Off ? SwitchState.On : SwitchState.Off;
                }
                await bulb.TransitionStateAsync(desiredState).ConfigureAwait(false);
            }
            return 0;
        }
    }
}
