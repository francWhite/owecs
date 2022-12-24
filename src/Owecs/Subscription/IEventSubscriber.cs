using System.Diagnostics.Eventing.Reader;

namespace Owecs.Subscription;

internal interface IEventSubscriber
{
	void Subscribe(Action<EventRecord> eventOccuredAction);
	void Unsubscribe();
}