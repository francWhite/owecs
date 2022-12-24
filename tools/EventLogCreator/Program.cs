using System.Diagnostics;

namespace EventLogCreator;

internal static class Program
{
	public static void Main(string[] args)
	{
		if (!OperatingSystem.IsWindows())
		{
			Console.WriteLine("only windows is supported");
			return;
		}

		const string eventLogSource = "test-app";
		const string logName = "ForwardedEvents";
		if (!EventLog.SourceExists(eventLogSource))
		{
			EventLog.CreateEventSource(eventLogSource, logName);	
		}

		using var eventLog = new EventLog(logName);
		eventLog.Source = eventLogSource;
		eventLog.MachineName = Environment.MachineName;
		

		var amount = int.Parse(args[0]);
		for (var i = 0; i < amount; i++)
		{
			eventLog.WriteEntry($"some message {Guid.NewGuid()}", EventLogEntryType.Warning, 3001);
			
		}
	}
}