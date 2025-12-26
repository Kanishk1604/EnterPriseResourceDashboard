namespace Erd.Api.Entities;

public enum AssetStatus { Available = 0, Assigned = 1, Retired = 2 }

public class Asset
{
    public int Id { get; set; }
    public string AssetTag { get; set; } = null!;
    public string AssetType { get; set; } = null!;
    public AssetStatus Status { get; set; } = AssetStatus.Available;

    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}