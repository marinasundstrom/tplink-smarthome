using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SHClient.Commands;
using SHClient.Services;

namespace SHClient
{
    [Command(Name = "shclient", Description = "Utility for controlling TP-Link Smart Home devices.")]
    [HelpOption("-?|--help|-h")]
    [Subcommand("discover", typeof(DiscoverCommand))]
    [Subcommand("bulb", typeof(BulbCommand))]
    [Subcommand("switch", typeof(SwitchCommand))]
#pragma warning disable RCS1102 // Make class static.
    internal class Program
#pragma warning restore RCS1102 // Make class static.
    {
        private static int Main(string[] args)
        {
            ServiceProvider provider = ConfigureServices();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(provider);

            return app.Execute(args);
        }

        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging(c =>
            {
                c.AddConsole();
                c.AddDebug();
            });

            services.AddSingleton<IJsonService, JsonService>();

            return services.BuildServiceProvider();
        }

        public int OnExecute()
        {
            return 0;
        }
    }
}
