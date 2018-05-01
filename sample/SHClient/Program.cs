using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SmartHome;
using SmartHome.Devices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SHClient
{
    static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Command("plug", (command) =>
            {
                command.Description = "Control a Smart Home plug.";
                command.HelpOption("-?|-h|--help");

                var targetArgument = command.Argument("[target]",
                                        "The address of the plug to control.");

                var stateOption = command.Option("-s|--state",
                                        "Sets the on/off state of the plug.", CommandOptionType.SingleValue);

                var jsonOption = command.Option("--json",
                                        "Output device info in JSON.", CommandOptionType.NoValue);

                command.OnExecute(async () =>
                {
                    var address = IPAddress.Parse(targetArgument.Value);

                    if (jsonOption.HasValue())
                    {
                        var plug = new Plug(address);
                        await plug.FetchAsync();
                        Console.WriteLine(JsonConvert.SerializeObject(plug, Formatting.Indented));
                    }
                    else
                    {
                        SwitchState desiredState = SwitchState.Off;

                        bool shouldToggleState = false;

                        if (string.Equals(stateOption.Value(), "toggle", StringComparison.InvariantCultureIgnoreCase))
                        {
                            shouldToggleState = true;
                        }
                        var plug = new Plug(address);
                        if (shouldToggleState)
                        {
                            await plug.FetchAsync();
                            desiredState = plug.RelayState == SwitchState.Off ? SwitchState.On : SwitchState.Off;
                        }
                        else if (!Enum.TryParse(stateOption.Value(), true, out desiredState))
                        {
                            throw new Exception("Invalid value for parameter \"state\".");
                        }
                        await plug.SetRelayStateAsync(desiredState);
                    }
                    return 0;
                });
            });
            app.Command("bulb", (command) =>
            {
                command.Description = "Control a Smart Home bulb.";
                command.HelpOption("-?|-h|--help");

                var targetArgument = command.Argument("[target]",
                                        "The address of the bulb to control.");

                var stateOption = command.Option("-s|--state",
                                        "Sets the on/off state of the light bulb.", CommandOptionType.SingleValue);

                var brightnessOption = command.Option("-b|--brightness",
                                        "Sets the brightness of the light bulb.", CommandOptionType.SingleValue);

                var jsonOption = command.Option("--json",
                                        "Output device info in JSON.", CommandOptionType.NoValue);

                command.OnExecute(async () =>
                {
                    var address = IPAddress.Parse(targetArgument.Value);

                    if (jsonOption.HasValue())
                    {
                        var bulb = new LightBulb(address);
                        await bulb.FetchAsync();
                        Console.WriteLine(JsonConvert.SerializeObject(bulb, Formatting.Indented));
                    }
                    else
                    {
                        var desiredState = new RequestedBulbState();

                        SwitchState desiredSwitchState = SwitchState.Off;
                        int desiredBrightness = 0;

                        bool shouldToggleState = false;

                        if (string.Equals(stateOption.Value(), "toggle", StringComparison.InvariantCultureIgnoreCase))
                        {
                            shouldToggleState = true;
                        }
                        else if (Enum.TryParse(stateOption.Value(), true, out desiredSwitchState))
                        {
                            desiredState.PowerState = desiredSwitchState;
                        }
                        else
                        {
                            throw new Exception("Invalid value for parameter \"state\".");
                        }
                        if (brightnessOption.HasValue())
                        {
                            if (int.TryParse(brightnessOption.Value(), out desiredBrightness))
                            {
                                desiredState.Brightness = desiredBrightness;
                            }
                            else
                            {
                                throw new Exception("Invalid value for parameter \"brightness\".");
                            }
                        }

                        var bulb = new LightBulb(address);
                        if (shouldToggleState)
                        {
                            await bulb.FetchAsync();
                            desiredState.PowerState = bulb.State.PowerState == SwitchState.Off ? SwitchState.On : SwitchState.Off;
                        }
                        await bulb.TransitionStateAsync(desiredState);
                    }
                    return 0;
                });
            });
            app.Command("discover", (command) =>
            {
                command.Description = "Watches for Smart Home devices on the network.";
                command.HelpOption("-?|-h|--help");

                var filterOption = command.Option("-f|--filter",
                                        "Sets the filter.", CommandOptionType.MultipleValue);

                var jsonOption = command.Option("--json",
                                        "Output device info in JSON.", CommandOptionType.NoValue);

                command.OnExecute(async () =>
                {
                    List<DeviceType> deviceFilter = null;

                    if (filterOption.HasValue())
                    {
                        DeviceType filterType;

                        if (Enum.TryParse(filterOption.Value(), true, out filterType))
                        {
                            if(deviceFilter == null) deviceFilter = new List<DeviceType>();

                            deviceFilter.Add(filterType);
                        }
                    }

                    using (var client = new SmartHomeClient() { DeviceTypeFilter = deviceFilter?.ToArray() })
                    {
                        client.DeviceDiscovered += (s, e) =>
                        {
                            if (jsonOption.HasValue())
                            {
                                Console.WriteLine(JsonConvert.SerializeObject(e.Device, Formatting.Indented));
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine($"{e.Device.Type,-20}{e.Device.Alias,-20}{e.Device.IPAddress} ");
                            }
                        };
                        client.DeviceUpdated += (s, e) =>
                        {
                            Console.WriteLine($"Device updated: {e.Device.Alias}");
                        };
                        client.Start();

                        while (true) await Task.Delay(1000);
                    }
                });
            });
            return app.Execute(args);
        }
    }
}
