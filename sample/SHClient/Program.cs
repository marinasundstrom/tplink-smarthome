using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SmartHome;
using SmartHome.Devices;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SHClient
{
    static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Command("plug", (Action<CommandLineApplication>)((command) =>
            {
                command.Description = "Control a Smart Home plug.";
                command.HelpOption("-?|-h|--help");

                var targetArgument = command.Argument("[target]",
                                        "The address of the plug to control.");

                var stateOption = command.Option("-s|--state",
                                        "Sets the on/off state of the plug.", CommandOptionType.SingleValue);

                command.OnExecute((Func<Task<int>>)(async () =>
                {
                    var address = IPAddress.Parse(targetArgument.Value);

                    SwitchState desiredState = SwitchState.Off;

                    if (Enum.TryParse<SwitchState>(stateOption.Value(), out desiredState))
                    {
                        var plug = new Plug(address);
                        //await plug.FetchAsync();
                        await plug.SetRelayStateAsync(desiredState);
                    }
                    return 0;
                }));
            }));
            app.Command("bulb", (Action<CommandLineApplication>)((command) =>
            {
                command.Description = "Control a Smart Home bulb.";
                command.HelpOption("-?|-h|--help");

                var targetArgument = command.Argument("[target]",
                                        "The address of the bulb to control.");

                var stateOption = command.Option("-s|--state",
                                        "Sets the on/off state of the light bulb.", CommandOptionType.SingleValue);

                var brightnessOption = command.Option("-b|--brightness",
                                        "Sets the brightness of the light bulb.", CommandOptionType.SingleValue);

                command.OnExecute((Func<Task<int>>)(async () =>
                {
                    var address = IPAddress.Parse(targetArgument.Value);

                    var state = new SmartHome.Devices.RequestedState();

                    SwitchState desiredState = SwitchState.Off;
                    int desiredBrightness = 0;

                    if (Enum.TryParse<SwitchState>(stateOption.Value(), out desiredState))
                    {
                        state.PowerState = desiredState;
                    }
                    if (int.TryParse(brightnessOption.Value(), out desiredBrightness))
                    {
                        state.Brightness = desiredBrightness;
                    }

                    var bulb = new LightBulb(address);
                    //await bulb.FetchAsync();
                    await bulb.TransitionStateAsync((RequestedState)state);
                    return 0;
                }));
            }));
            app.Command("discover", (Action<CommandLineApplication>)((command) =>
            {
                command.Description = "Watches for Smart Home devices on the network.";
                command.HelpOption("-?|-h|--help");

                /*
                var stateOption = command.Option("-f|--filter",
                                        "Sets the filter.", CommandOptionType.SingleValue);
                */
                
                var jsonOption = command.Option("--json",
                                        "Output device info in JSON.", CommandOptionType.NoValue);


                command.OnExecute((Func<Task<int>>)(async () =>
                {
                    using (var client = new SmartHomeClient())
                    {
                        client.DeviceDiscovered += (s, e) =>
                        {
                            if(jsonOption.HasValue()) {
                                 Console.WriteLine(JsonConvert.SerializeObject(e.Device, Formatting.Indented));
                                Console.WriteLine();
                            } 
                            else 
                            {
                                Console.WriteLine($"{e.Device.Type, -20}{e.Device.Alias, -20}{e.Device.IPAddress} ");
                            }
                        };
                        client.Start();

                        while (true) await Task.Delay(1000);
                    }
                    return 0;
                }));
            }));
            return app.Execute(args);
        }

        private static async Task Main2()
        {
            using (var client = new SmartHomeClient())
            {
                client.Start();

                await Task.Delay(1000);

                Console.WriteLine("Client initialized.");

                while (true)
                {
                    Console.Write("Command: ");
                    var command = Console.ReadLine();
                    if (command?.ToLower() == "exit") return;
                    foreach (var device in client.GetDevices())
                    {
                        if (device is LightBulb lb)
                        {
                            var state = string.Equals(command, "x", StringComparison.CurrentCultureIgnoreCase) ? SwitchState.On : SwitchState.Off;

                            await lb.TransitionStateAsync(state);

                            var state2 = await lb.GetStateAsync();

                            Console.WriteLine($"{lb.Alias} ({lb.DeviceId}): {state2.PowerState}");
                        }
                    }
                }
            }
        }

        static void Test()
        {
            using (var client = new SmartHomeClient())
            {
                client.DeviceDiscovered += (s, e) =>
                {
                    //Console.WriteLine($"{DateTime.Now}: {e.Device.DeviceId}");
                    Console.WriteLine(JsonConvert.SerializeObject(e.Device, Formatting.Indented));
                    Console.WriteLine();
                };
                client.Start();

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

        async static Task Test2()
        {
            using (var client = new SmartHomeClient())
            {
                client.Start();

                await Task.Delay(1000);

                var devices = client
                       .GetDevices();
                var bulb = devices.OfType<LightBulb>().First();

                while (true)
                {
                    Console.Write($"Brightness ({bulb.State.Brightness}): ");
                    var value = Console.ReadLine();
                    if (value.ToLower() == "exit") break;
                    await bulb.TransitionStateAsync(new RequestedState()
                    {
                        PowerState = SwitchState.On,
                        Brightness = int.Parse(value)
                    });
                    Console.Clear();
                }

                await bulb.TransitionStateAsync(SwitchState.Off);

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

        async static Task Test3()
        {
            using (var client = new SmartHomeClient())
            {
                client.Start();

                await Task.Delay(1000);

                var devices = client
                       .GetDevices();
                var bulb = devices.OfType<Plug>().First();

                while (true)
                {
                    Console.Write($"State ({await bulb.GetRelayStateAsync()}): ");
                    var value = Console.ReadLine();
                    if (value.ToLower() == "exit") break;
                    if (bool.TryParse(value, out var flag))
                    {
                        await bulb.SetRelayStateAsync(flag ? SwitchState.On : SwitchState.Off);
                    }
                    Console.Clear();
                }

                await bulb.SetRelayStateAsync(SwitchState.Off);

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

        async static Task Test4(string[] args)
        {
            var desiredState = Enum.Parse<SwitchState>(args[0], true);

            var bulb = new LightBulb(IPAddress.Parse("192.168.1.7"));
            //await bulb.FetchAsync();
            await bulb.TransitionStateAsync(desiredState);
        }
    }
}
