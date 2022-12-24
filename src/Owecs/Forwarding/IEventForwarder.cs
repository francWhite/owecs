using System.Diagnostics.Eventing.Reader;

namespace Owecs.Forwarding;

internal interface IEventForwarder
{
	Task<bool> IsAvailableAsync();
	Task InitializeAsync();
	Task ForwardAsync(IEnumerable<EventRecord> eventRecords, CancellationToken cancellationToken);
}