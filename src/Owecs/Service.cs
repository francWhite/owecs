using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Owecs;

public class Service : BackgroundService
{
    private readonly ILogger<Service> _logger;

    public Service(ILogger<Service> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
	    try
	    {
		    while (!stoppingToken.IsCancellationRequested)
		    {
			    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
			    await Task.Delay(1000, stoppingToken);
		    }
	    }
	    catch (Exception e)
	    {
		    _logger.LogError(e, "{Message}", e.Message);
		    Environment.Exit(1);
	    }
    }
}
