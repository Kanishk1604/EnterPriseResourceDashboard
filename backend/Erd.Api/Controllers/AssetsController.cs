using Erd.Api.Data;
using Erd.Api.DTOs;
using Erd.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Erd.Api.Controllers;

[ApiController]
[Route("assets")]
[Authorize] // everything here requires authentication
public class AssetsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssetsController(AppDbContext db)
    {
        _db = db;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAsset(CreateAssetRequest request)
    {
        var asset = new Asset
        {
            Name = request.Name,
            AssetTag = request.AssetTag,
            AssetType = request.AssetType
        };

        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();

        return Ok(asset);
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("{assetId}/assign")]
    public async Task<IActionResult> AssignAsset(int assetId, AssignAssetRequest request)
    {
        var asset = await _db.Assets.FindAsync(assetId);
        if (asset == null)
            return NotFound("Asset not found");

        var user = await _db.Users.FindAsync(request.UserId);
        if (user == null)
            return NotFound("User not found");

        asset.AssignedToUserId = user.Id;
        await _db.SaveChangesAsync();

        return Ok("Asset assigned");
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAssets()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var assets = await _db.Assets
            .Where(a => a.AssignedToUserId == userId)
            .ToListAsync();

        return Ok(assets);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset == null)
            return NotFound();

        _db.Assets.Remove(asset);
        await _db.SaveChangesAsync();

        return NoContent();
    }

}