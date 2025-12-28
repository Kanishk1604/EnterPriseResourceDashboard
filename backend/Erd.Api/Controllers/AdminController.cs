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
    public async Task<IActionResult> GetAuditLogs()
    {
        var logs = await _db.AuditLogs
            .OrderByDescending(a => a.TimestampUtc)
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

        return Ok(logs);
    }
}