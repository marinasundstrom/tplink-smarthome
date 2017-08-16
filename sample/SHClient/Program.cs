using Newtonsoft.Json;
using SmartHome;
using SmartHome.Devices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SHClient
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            //await Main2();
            Test();
            //await Test2();
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

                            state = await lb.GetStateAsync();

                            Console.WriteLine($"{lb.Alias} ({lb.DeviceId}): {state}");
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
                    Console.Write($"Brightness ({bulb.Parameters.Brightness}): ");
                    var value = Console.ReadLine();
                    if(value.ToLower() == "exit") break;
                    await bulb.TransitionStateAsync(SwitchState.On, brightness: int.Parse(value));
                    Console.Clear();
                }

                await bulb.TransitionStateAsync(SwitchState.Off);

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }
    }
}
