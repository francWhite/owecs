using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;

namespace Owecs.Forwarding;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal class EventForwarder : IEventForwarder
{
	private readonly ILogger<EventForwarder> _logger;
	private readonly IForwardingAdapter _forwardingAdapter;

	public EventForwarder(ILogger<EventForwarder> logger,
		IConfiguration configuration,
		IEnumerable<IForwardingAdapter> forwardingAdapters)
	{
		_logger = logger;

		//ToDo log error if adapter is not found
		var adapterKey = configuration.GetValue<string>("Forwarding:Type");
		_forwardingAdapter = forwardingAdapters.Single(adapter => adapter.Key == adapterKey);
	}

	public async Task<bool> IsAvailableAsync()
	{
		return await _forwardingAdapter.IsAvailableAsync();
	}

	public async Task InitializeAsync()
	{
		await _forwardingAdapter.InitializeAsync();
	}

	public async Task ForwardAsync(IEnumerable<EventRecord> eventRecords, CancellationToken cancellationToken)
	{
		var eventRecordDtos = eventRecords.Select(CreateEventRecordDto).ToList();
		await _forwardingAdapter.ForwardAsync(eventRecordDtos, cancellationToken);
	}
	
	private static EventRecordDto CreateEventRecordDto(EventRecord eventRecord)
	{
		return new EventRecordDto
		{
			Id = eventRecord.Id,
			ActivityId = eventRecord.ActivityId,
			TimeCreated = eventRecord.TimeCreated,
			LogName = eventRecord.LogName,
			Version = eventRecord.Version,
			Qualifiers = eventRecord.Qualifiers,

			Level = eventRecord.Level,
			LevelDisplayName = eventRecord.LevelDisplayName,
			ProviderId = eventRecord.ProviderId,
			ProviderName = eventRecord.ProviderName,
			Opcode = eventRecord.Opcode,
			OpcodeDisplayName = eventRecord.OpcodeDisplayName,
			ContainerLog = (eventRecord as EventLogRecord)?.ContainerLog,

			MachineName = eventRecord.MachineName,
			UserSid = eventRecord.UserId?.ToString(),

			ProcessId = eventRecord.ProcessId,
			ThreadId = eventRecord.ThreadId,
			Task = eventRecord.Task,

			EventProperties = eventRecord.Properties.Select(p => p.Value.ToString()).ToList(),
			Xml = eventRecord.ToXml(),
		};
	}
}