using Owecs.Forwarding;
using Owecs.Subscription;

namespace Owecs;

public class Program
{
	public static async Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.UseWindowsService(options => options.ServiceName = "Open Windows Event Forwarder")
			.ConfigureServices(RegisterServices)
			.Build();

		var logger = host.Services.GetService<ILogger<Program>>();
		if (!OperatingSystem.IsWindows())
		{
			logger?.LogError("This service can only be run on a windows system");
			return;
		}

		await host.RunAsync();
	}

	private static void RegisterServices(IServiceCollection services)
	{
		services
			.AddHostedService<Service>()
			.AddForwardingAdapters()
			.AddSingleton<IEventForwarder, EventForwarder>()
			.AddSingleton<IEventSubscriber, EventSubscriber>();
	}
}