using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHome;
using SmartHome.Devices;

namespace TestApp
{
    internal static class Program
    {
        private static void Main()
        {
            Test();
        }

#pragma warning disable RCS1213 // Remove unused member declaration.
        private static async Task Main2()
#pragma warning restore RCS1213 // Remove unused member declaration.
        {
            using (var client = new SmartHomeClient())
            {
                client.Start();

                await Task.Delay(1000).ConfigureAwait(false);

                Console.WriteLine("Client initialized.");

                while (true)
                {
                    Console.Write("Command: ");
                    string command = Console.ReadLine();
                    if (command?.ToLower() == "exit")
                    {
                        return;
                    }

                    foreach (Device device in client.GetDevices())
                    {
                        if (device is LightBulb lb)
                        {
                            SwitchState state = string.Equals(command, "x", StringComparison.CurrentCultureIgnoreCase) ? SwitchState.On : SwitchState.Off;

                            await lb.TransitionStateAsync(state).ConfigureAwait(false);

                            LightBulbState state2 = lb.State;

                            Console.WriteLine($"{lb.Alias} ({lb.DeviceId}): {state2.PowerState}");
                        }
                    }
                }
            }
        }

        private static void Test()
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

#pragma warning disable RCS1213 // Remove unused member declaration.
        private static async Task Test2()
#pragma warning restore RCS1213 // Remove unused member declaration.
        {
            using (var client = new SmartHomeClient())
            {
                client.Start();

                await Task.Delay(1000).ConfigureAwait(false);

                System.Collections.Generic.IEnumerable<IDevice> devices = client
                       .GetDevices();
                LightBulb bulb = devices.OfType<LightBulb>().First();

                while (true)
                {
                    Console.Write($"Brightness ({bulb.State.Brightness}): ");
                    string value = Console.ReadLine();
                    if (string.Equals(value, "exit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }

                    await bulb.TransitionStateAsync(new RequestedBulbState()
                    {
                        PowerState = SwitchState.On,
                        Brightness = int.Parse(value)
                    }).ConfigureAwait(false);
                    Console.Clear();
                }

                await bulb.TransitionStateAsync(SwitchState.Off).ConfigureAwait(false);

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

#pragma warning disable RCS1213 // Remove unused member declaration.
        private static async Task Test3()
#pragma warning restore RCS1213 // Remove unused member declaration.
        {
            using (var client = new SmartHomeClient())
            {
                client.Start();

                await Task.Delay(1000).ConfigureAwait(false);

                System.Collections.Generic.IEnumerable<IDevice> devices = client
                       .GetDevices();
                Plug bulb = devices.OfType<Plug>().First();

                while (true)
                {
                    Console.Write($"State ({bulb.RelayState}): ");
                    string value = Console.ReadLine();
                    if (string.Equals(value, "exit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }

                    if (bool.TryParse(value, out bool flag))
                    {
                        await bulb.SetRelayStateAsync(flag ? SwitchState.On : SwitchState.Off).ConfigureAwait(false);
                    }
                    Console.Clear();
                }

                await bulb.SetRelayStateAsync(SwitchState.Off).ConfigureAwait(false);

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

#pragma warning disable RCS1213 // Remove unused member declaration.
        private static async Task Test4(string[] args)
#pragma warning restore RCS1213 // Remove unused member declaration.
        {
            SwitchState desiredState = Enum.Parse<SwitchState>(args[0], true);

            var bulb = new LightBulb(IPAddress.Parse("192.168.1.7"));
            //await bulb.FetchAsync();
            await bulb.TransitionStateAsync(desiredState).ConfigureAwait(false);
        }
    }
}
