using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using SHClient.Services;
using SmartHome;
using SmartHome.Devices;

namespace SHClient.Commands
{
    [Command("discover", Description = "Discover devices on the network."), HelpOption]
    public class DiscoverCommand
    {
        private readonly ILogger<DiscoverCommand> _logger;
        private readonly IJsonService _jsonService;

        public DiscoverCommand(ILogger<DiscoverCommand> logger, IJsonService jsonService)
        {
            _logger = logger;
            _jsonService = jsonService;
        }

        [Option("--filter|-f", Description = "Sets the filter.")]
        public string Filter { get; }

        [Option("--json|-j", Description = "Output device info in JSON.")]
        public bool ShowJson { get; }

        public int OnExecute(IConsole console)
        {
            DeviceType[] deviceFilter = null;

            if (Filter != null)
            {
                if (Enum.TryParse(Filter, true, out DeviceType filterType))
                {
                    deviceFilter = new[] { filterType };
                }
            }

            using (var client = new SmartHomeClient(deviceFilter))
            {
                client.DeviceDiscovered += (s, e) =>
                {
                    if (ShowJson)
                    {
                        console.WriteLine(_jsonService.Serialize(e.Device));
                        console.WriteLine();
                    }
                    else
                    {
                        console.WriteLine($"{e.Device.Type,-20}{e.Device.Alias,-20}{e.Device.IPAddress} ");
                    }
                };
                client.DeviceUpdated += (s, e) => Console.WriteLine($"Device updated: {e.Device.Alias}");
                client.Start();

                Console.ReadKey();

                return 0;
            }
        }
    }
}
