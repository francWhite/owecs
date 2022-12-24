using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Owecs.Forwarding.Adapter.Kafka;

internal class KafkaForwardingAdapter : IForwardingAdapter
{
	private readonly ILogger<KafkaForwardingAdapter> _logger;
	private readonly IConfigurationSection _configuration;
	private ProducerConfig? _producerConfig;
	private string? _topic;

	public KafkaForwardingAdapter(IConfiguration configuration, ILogger<KafkaForwardingAdapter> logger)
	{
		_logger = logger;
		_configuration = configuration.GetRequiredSection("Forwarding:Configuration");
	}

	public string Key => "Adapter.Kafka";

	public Task InitializeAsync()
	{
		var bootstrapServers = _configuration.GetSection("BootstrapServers").Get<string[]>();
		if (bootstrapServers == null)
		{
			throw new InvalidOperationException("configuration 'BootstrapServers' must be defined");
		}

		//ToDo ensure that topic really exists
		_topic = _configuration.GetValue<string>("Topic");
		if (string.IsNullOrWhiteSpace(_topic))
		{
			throw new InvalidOperationException("configuration 'Topic' must be defined");
		}

		_producerConfig = new ProducerConfig
		{
			BootstrapServers = bootstrapServers.Aggregate((a, b) => a + "," + b)
		};

		return Task.CompletedTask;
	}

	public Task<bool> IsAvailableAsync()
	{
		//ToDo implement actual check
		return Task.FromResult(true);
	}

	public Task ForwardAsync(IEnumerable<EventRecordDto> eventRecordDtos, CancellationToken cancellationToken)
	{
		using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
		foreach (var eventRecordDto in eventRecordDtos)
		{
			producer.Produce(
				_topic!,
				CreateMessage(eventRecordDto),
				deliveryReport =>
				{
					if (deliveryReport.Error.Code != ErrorCode.NoError)
					{
						//ToDo handle failed deliveries
					}
				});
		}

		producer.Flush(cancellationToken);
		return Task.CompletedTask;
	}


	private static Message<Null, string> CreateMessage(EventRecordDto eventRecordDto)
	{
		var serializedEventRecord = JsonSerializer.Serialize(eventRecordDto);
		return new Message<Null, string>
		{
			Value = serializedEventRecord
		};
	}
}