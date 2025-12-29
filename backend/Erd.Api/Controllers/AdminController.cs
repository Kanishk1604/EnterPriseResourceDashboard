using Erd.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erd.Api.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
        int page = 1,
        int pageSize = 10,
        string? action = null,
        string? entityType = null
    )
    {
        pageSize = Math.Min(pageSize, 50);

        var query = _db.AuditLogs.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action == action);

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(a => a.TimestampUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.EntityType,
                a.EntityId,
                a.Action,
                a.PerformedBy,
                a.TimestampUtc
            })
            .ToListAsync();

        return Ok(new 
        {
            page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            data = logs
        });
    }
}