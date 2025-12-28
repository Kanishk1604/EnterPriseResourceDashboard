namespace Erd.Api.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string EntityType { get; set; } = null!;
    public int EntityId { get; set; }
    public string Action { get; set; } = null!;
    public int PerformedBy { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}