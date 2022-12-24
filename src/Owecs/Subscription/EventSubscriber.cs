using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;

namespace Owecs.Subscription;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal class EventSubscriber : IEventSubscriber
{
	private readonly EventLogWatcher _watcher;
	private Action<EventRecord>? _eventOccuredAction;

	public EventSubscriber(IConfiguration configuration, ILogger<EventSubscriber> logger)
	{
		var eventSource = configuration.GetValue<string>("Application:EventSource") ?? "ForwardedEvents";
		logger.LogInformation("listening for events in source {source}", eventSource);
		
		var query = new EventLogQuery(eventSource, PathType.LogName);
		_watcher = new EventLogWatcher(query);
	}

	public void Subscribe(Action<EventRecord> eventOccuredAction)
	{
		_eventOccuredAction = eventOccuredAction;
		_watcher.EventRecordWritten += OnEventRecordWritten;
		_watcher.Enabled = true;
	}

	public void Unsubscribe()
	{
		_watcher.Enabled = false;
		_watcher.EventRecordWritten -= OnEventRecordWritten;
	}

	private void OnEventRecordWritten(object? sender, EventRecordWrittenEventArgs e)
	{
		_eventOccuredAction?.Invoke(e.EventRecord);
	}
}