namespace Erd.Api.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string EntityType { get; set; } = null!;
    public int EntityId { get; set; }
    public string Action { get; set; } = null!;
    public string PerformedBy { get; set; } = null!;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}