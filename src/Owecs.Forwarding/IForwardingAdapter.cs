namespace Owecs.Forwarding;

public interface IForwardingAdapter
{
	public string Key { get; }
	Task InitializeAsync();
	Task<bool> IsAvailableAsync();
	Task ForwardAsync(IEnumerable<EventRecordDto> eventRecordDtos, CancellationToken cancellationToken);
}