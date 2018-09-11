using System;
using System.Net;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using SHClient.Services;
using SmartHome.Devices;

namespace SHClient.Commands
{
    [Command("switch", Description = "Control a switch."), HelpOption]
    public class SwitchCommand
    {
        private readonly ILogger<SwitchCommand> _logger;
        private readonly IJsonService _jsonService;

        public SwitchCommand(ILogger<SwitchCommand> logger, IJsonService jsonService)
        {
            _logger = logger;
            _jsonService = jsonService;
        }

        [Argument(0, Description = "The address of the plug to control.")]
        public string TargetDevice { get; }

        [Option("--state|-s", Description = "Sets the on/off state of the plug.")]
        public string DesiredState { get; }

        [Option("--json|-j", Description = "Output device info in JSON.")]
        public bool ShowJson { get; }

        public async Task<int> OnExecuteAsync(IConsole console)
        {
            var address = IPAddress.Parse(TargetDevice);

            if (ShowJson)
            {
                var plug = new Plug(address);
                await plug.FetchAsync().ConfigureAwait(false);
                console.WriteLine(_jsonService.Serialize(plug));
            }
            else
            {
                SwitchState desiredState = SwitchState.Off;

                bool shouldToggleState = false;

                if (string.Equals(DesiredState, "toggle", StringComparison.InvariantCultureIgnoreCase))
                {
                    shouldToggleState = true;
                }
                var plug = new Plug(address);
                if (shouldToggleState)
                {
                    await plug.FetchAsync().ConfigureAwait(false);
                    desiredState = plug.RelayState == SwitchState.Off ? SwitchState.On : SwitchState.Off;
                }
                else if (!Enum.TryParse(DesiredState, true, out desiredState))
                {
                    throw new Exception("Invalid value for parameter \"state\".");
                }
                await plug.SetRelayStateAsync(desiredState).ConfigureAwait(false);
            }

            return 0;
        }
    }
}
