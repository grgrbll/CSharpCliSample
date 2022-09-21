using System;
using SampleConsoleApp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
namespace Tests
{
	public class ConsoleRunner
	{
		static public ConsoleRunner Run(string args)
		{
			var runner = RunAsync(args); 
			runner.WaitComplete();
			return runner;
		}

		static public ConsoleRunner RunAsync(string args)
		{
			return new ConsoleRunner(args.Split());
		}


		TextWriter output = new StringWriter();
		Task hostedService;

		private ConsoleRunner(IEnumerable<string> args)
		{
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<TextWriter>(output);
                services.AddHostedService(serviceProvider => 
		            {
                        return new ConsoleHostedService(args, serviceProvider.GetService<IServiceProvider>());
		            });
            });
            var host = builder.Build();
            hostedService = host.RunAsync();
		}

		public string GetOutput()
		{
			lock (output)
			{
				return output.ToString();
			}
		}

		public void WaitComplete(int timeoutMs = -1, CancellationToken? cancel = null)
		{
			if (cancel != null)
				hostedService.Wait(timeoutMs, cancel.Value);
			else
				hostedService.Wait(timeoutMs);
		}
	}
}

