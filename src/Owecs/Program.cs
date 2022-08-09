using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Owecs;

var host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
    {
        services.AddHostedService<Service>();
    })
    .Build();

await host.RunAsync();
