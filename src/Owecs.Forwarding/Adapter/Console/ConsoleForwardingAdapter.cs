namespace Owecs.Forwarding.Adapter.Console;

internal class ConsoleForwardingAdapter : IForwardingAdapter
{
	public string Key => "Adapter.Console";

	public Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public Task<bool> IsAvailableAsync()
	{
		return Task.FromResult(true);
	}

	public async Task ForwardAsync(IEnumerable<EventRecordDto> eventRecordDtos, CancellationToken cancellationToken)
	{
		await Parallel.ForEachAsync(
			eventRecordDtos,
			cancellationToken,
			(eventRecordDto, _) =>
			{
				System.Console.WriteLine($"received event: {eventRecordDto.LevelDisplayName} - {eventRecordDto.ActivityId} - {eventRecordDto.MachineName} - {eventRecordDto.EventProperties.Aggregate((a, b) => a + ", " + b)}");
				return ValueTask.CompletedTask;
			}
		);
	}
}