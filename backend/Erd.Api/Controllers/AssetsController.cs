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

        await LogAudit(
            action: "ASSET_CREATED",
            entity: "Asset",
            entityId: asset.Id
        );

        return Ok(asset);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    public async Task<IActionResult> GetAssets(
        int page = 1,
        int pageSize = 10,
        string? assetType = null,
        bool? assigned = null
    )
    {
        pageSize = Math.Min(pageSize, 50);

        var query = _db.Assets.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(assetType))
            query = query.Where(a => a.AssetType == assetType);

        if (assigned.HasValue)
            query = assigned.Value
                ? query.Where(a => a.AssignedToUserId != null)
                : query.Where(a => a.AssignedToUserId == null);

        var totalCount = await query.CountAsync();

        var assets = await query
            .OrderByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.Name,
                a.AssetTag,
                a.AssetType,
                a.AssignedToUserId
            })
            .ToListAsync();

        return Ok(new
        {
            page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            data = assets
        });
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

        var previousUserId = asset.AssignedToUserId;

        asset.AssignedToUserId = user.Id;
        await _db.SaveChangesAsync();

        await LogAudit(
            action: previousUserId == null ? "ASSET_ASSIGNED" : "ASSET_REASSIGNED",
            entity: "Asset",
            entityId: asset.Id
        );
        return Ok("Asset assigned");
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("{assetId}/unassign")]
    public async Task<IActionResult> UnassignAsset(int assetId)
    {
        var asset = await _db.Assets.FindAsync(assetId);
        if (asset == null)
            return NotFound("Asset not found");

        if(asset.AssignedToUserId == null){
            return BadRequest("Asset is already unassigned");
        }

        asset.AssignedToUserId = null;
        await _db.SaveChangesAsync();

        await LogAudit(
            action: "Asset_UNASSIGNED",
            entity: "Asset",
            entityId: assetId
        );

        return Ok("Asset Unassigned");
        
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

    //private method to log audit actions
    private async Task LogAudit(string action, string entity, int entityId)
    {   
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var log = new AuditLog
        {
            Action = action,
            EntityType = entity,
            EntityId = entityId,
            PerformedBy = User.FindFirstValue(ClaimTypes.Email)!
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }   

    
}