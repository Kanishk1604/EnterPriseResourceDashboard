using Erd.Api.Auth;
using Erd.Api.Data;
using Erd.Api.DTOs;
using Erd.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Erd.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwordService;
    private readonly JwtTokenService _jwtService;

    public AuthController(
        AppDbContext db,
        PasswordService passwordService,
        JwtTokenService jwtService)
    {
        _db = db;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var role = await _db.Roles.FindAsync(request.RoleId);
        if (role == null)
            return BadRequest("Invalid role");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            RoleId = role.Id
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return Unauthorized("Invalid credentials");

        if (!_passwordService.VerifyPassword(user.PasswordHash, request.Password))
            return Unauthorized("Invalid credentials");

        var token = _jwtService.GenerateToken(
            user.Id,
            user.Email,
            user.Role.Name);

        return Ok(new { token });
    }

    //verification and debugging endpoint
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role),
            isAuthenticated = User.Identity?.IsAuthenticated
        });
    }
}