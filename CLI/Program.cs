// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Extensions.Hosting;
using GbUtil;

namespace SampleConsoleApp
{

    public class ConsoleHostedService : IHostedService
    {
        private IEnumerable<Type> Plugins;
        private List<string> Args;
        private IServiceProvider _serviceProvider { get; set; }

        public ConsoleHostedService(IEnumerable<string> args, IServiceProvider serviceProvider)
        {
            Args = args.ToList();
            Plugins = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(PluginDefinitionAttribute)));
            _serviceProvider = serviceProvider;
        }

        void PrintHelp()
        {

        }

        void PrintGroupHelp(string groupName)
        {

        }

        Task ExecutePlugin(Type pluginType, IEnumerable<string> args)
        {
            Plugin? plugin = Activator.CreateInstance(pluginType) as Plugin;
            if (plugin == null)
                throw new ApplicationException("Bad plugin type. Must inherit from Plugin.");

            //plugin.OutputWriter = _serviceProvider?.GetService<TextWriter>();
            return plugin.ExecuteWithArgs(args, _serviceProvider);
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            using (new ScopeGuard(delegate { _serviceProvider?.GetService<IHostApplicationLifetime>()?.StopApplication(); }))
            {
                if (Args.Count() < 1)
                {
                    PrintHelp();
                    return;
                }

                string group = Args[0];

                if (Args.Count() < 2)
                {
                    PrintGroupHelp(Args[1]);
                    return;
                }

                string cmd = Args[1];

                foreach (Type pluginType in Plugins)
                {
                    var pluginDefinition = pluginType.GetCustomAttribute<PluginDefinitionAttribute>();

                    if (pluginDefinition == null)
                        throw new ApplicationException("Bad plugin type. Must inherit from Plugin.");

                    if (pluginDefinition.Group == group && pluginDefinition.Name == cmd)
                        await ExecutePlugin(pluginType, Args.Skip(2));
                }

                return;
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            // This gets called after IHostApplicationLifetime.StopApplication()
            return Task.CompletedTask;
        }
    }

    class SampleConsoleApp
    {
        static async Task<int> Main(string[] args)
        {
			var serilogConf = new LoggerConfiguration();
			serilogConf.WriteTo.Console();
			Log.Logger = serilogConf.CreateLogger();

            Log.Information("Building host.");
            var builder = new HostBuilder()
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<TextWriter>(Console.Out);
                services.AddHostedService(serviceProvider => 
		            {
                        return new ConsoleHostedService(args, serviceProvider.GetService<IServiceProvider>());
		            });
            });
            var host = builder.Build();

            Log.Information("Running host.");
            host.Run();

            return 0;
        }
    }
}