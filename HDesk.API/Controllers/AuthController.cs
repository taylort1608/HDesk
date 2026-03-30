using System;
using HDesk.API.Extensions;
using HDesk.Core.DTOs;
using HDesk.Core.Interfaces;
using HDesk.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository users, IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (await users.EmailExistsAsync(registerDto.Email))
            return Conflict(new { error = "Email is already registered."});

        var user = new User
        {
            FullName = registerDto.FullName,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = UserRoles.EndUser
        };

        await users.CreateAsync(user);

        var token = JwtHelper.GenerateToken(user, configuration);
        return Ok(new AuthResponseDto(token, user.FullName, user.Email, user.Role.ToString()));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await users.GetByEmailAsync(loginDto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid email or password."});

        if (!user.IsActive)
            return Unauthorized(new { error = "Account is disabled."});

        var token = JwtHelper.GenerateToken(user, configuration);
        return Ok(new AuthResponseDto(token, user.FullName, user.Email, user.Role.ToString()));
    }
}
