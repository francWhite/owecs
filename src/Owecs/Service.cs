using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using Owecs.Forwarding;
using Owecs.Subscription;

namespace Owecs;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal class Service : BackgroundService
{
	private readonly ILogger<Service> _logger;
	private readonly IEventForwarder _eventForwarder;
	private readonly IEventSubscriber _eventSubscriber;
	private readonly Stopwatch _stopwatch = new();

	private readonly ConcurrentQueue<EventRecord> _eventRecordsQueue = new();
	private readonly int _forwardingIntervalInMs;

	public Service(ILogger<Service> logger,
		IEventForwarder eventForwarder,
		IEventSubscriber eventSubscriber,
		IConfiguration configuration)
	{
		_logger = logger;
		_eventForwarder = eventForwarder;
		_eventSubscriber = eventSubscriber;
		_forwardingIntervalInMs = configuration.GetValue<int>("Application:ForwardingIntervalInMs");
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		try
		{
			await _eventForwarder.InitializeAsync();
			_eventSubscriber.Subscribe(_eventRecordsQueue.Enqueue);
			_logger.LogInformation("Service started, waiting for events");

			while (!cancellationToken.IsCancellationRequested)
			{
				await TryForwardEventRecordsAsync(cancellationToken);
				await Task.Delay(_forwardingIntervalInMs, cancellationToken);
			}

			_eventSubscriber.Unsubscribe();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Unexpected error occured, terminating service");
			Environment.Exit(1);
		}
	}

	private async Task TryForwardEventRecordsAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		_stopwatch.Reset();
		_stopwatch.Start();

		var eventRecords = DequeueEventRecords().ToList();
		if (!eventRecords.Any())
		{
			return;
		}

		var forwarderIsAvailable = await _eventForwarder.IsAvailableAsync();
		if (!forwarderIsAvailable)
		{
			_logger.LogWarning("forwarder is not available");
			eventRecords.ForEach(_eventRecordsQueue.Enqueue);
			return;
		}


		try
		{
			await _eventForwarder.ForwardAsync(eventRecords, cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error occured while forwarding events!");
			eventRecords.ForEach(_eventRecordsQueue.Enqueue);
		}

		_stopwatch.Stop();
		_logger.LogInformation("forwarding of {events} events took {time}ms", eventRecords.Count, _stopwatch.ElapsedMilliseconds);
	}

	private ConcurrentBag<EventRecord> DequeueEventRecords()
	{
		_logger.LogDebug("queue size: {size}", _eventRecordsQueue.Count);

		var eventRecords = new ConcurrentBag<EventRecord>();
		while (_eventRecordsQueue.TryDequeue(out var eventRecord))
		{
			eventRecords.Add(eventRecord);
		}

		_logger.LogDebug("dequeued {amount} items for forwarding", eventRecords.Count);
		return eventRecords;
	}
}