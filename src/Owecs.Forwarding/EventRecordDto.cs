namespace Owecs.Forwarding;

public struct EventRecordDto
{
	public int Id { get; set; }
	public Guid? ActivityId { get; set; }
	public DateTime? TimeCreated { get; set; }
	public string? LogName { get; set; }
	public byte? Version { get; set; }
	public int? Qualifiers { get; set; }

	public byte? Level { get; set; }
	public string? LevelDisplayName { get; set; }
	public Guid? ProviderId { get; set; }
	public string? ProviderName { get; set; }
	public short? Opcode { get; set; }
	public string? OpcodeDisplayName { get; set; }
	public string? ContainerLog { get; set; }

	public string? MachineName { get; set; }
	public string? UserSid { get; set; }

	public int? ProcessId { get; set; }
	public int? ThreadId { get; set; }
	public int? Task { get; set; }

	public List<string?> EventProperties { get; set; }
	public string? Xml { get; set; }
}