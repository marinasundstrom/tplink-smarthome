using Newtonsoft.Json;
using SmartHome;
using SmartHome.Devices;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Test();
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

                            var state2 = lb.State;

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
                    if (string.Equals(value, "exit", StringComparison.CurrentCultureIgnoreCase)) break;
                    await bulb.TransitionStateAsync(new RequestedBulbState()
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
                    Console.Write($"State ({bulb.RelayState}): ");
                    var value = Console.ReadLine();
                    if (string.Equals(value, "exit", StringComparison.CurrentCultureIgnoreCase)) break;
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
