using Microsoft.Extensions.DependencyInjection;
using Owecs.Forwarding.Adapter.Console;
using Owecs.Forwarding.Adapter.Kafka;

namespace Owecs.Forwarding;

public static class ForwardingServiceCollectionExtension
{
	public static IServiceCollection AddForwardingAdapters(this IServiceCollection services)
	{
		return services
			.AddSingleton<IForwardingAdapter, KafkaForwardingAdapter>()
			.AddSingleton<IForwardingAdapter, ConsoleForwardingAdapter>();
	}
}