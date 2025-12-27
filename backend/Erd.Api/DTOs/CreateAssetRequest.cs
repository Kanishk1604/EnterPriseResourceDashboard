namespace Erd.Api.DTOs;

public class CreateAssetRequest
{
    public string Name { get; set; } = null!;
    public string AssetTag { get; set; } = null!;
    public string AssetType { get; set; } = null!;
}