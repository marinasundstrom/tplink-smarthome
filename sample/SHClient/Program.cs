using Newtonsoft.Json;
using SmartHome;
using SmartHome.Devices;
using System;
using System.Threading.Tasks;

namespace SHClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test();

            MainAsync().Wait();
        }

        private static async Task MainAsync()
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
                            await lb.SetTransitionStateAsync(command == "X" ? SwitchState.On : SwitchState.Off, 0);

                            var state = await lb.GetStateAsync();

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
                client.DeviceDiscovered += async (s, e) =>
                {
                    Console.WriteLine(JsonConvert.SerializeObject(e.Device, Formatting.Indented));
                    Console.WriteLine();
                };
                client.Start();

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }
    }
}
