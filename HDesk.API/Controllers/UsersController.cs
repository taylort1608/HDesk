using System;
using HDesk.API.Extensions;
using HDesk.Core.DTOs;
using HDesk.Core.Interfaces;
using HDesk.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserRepository users) : ControllerBase
{
    // Admin only - List all users
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var all = await users.GetAllAsync();
        var dtos = all.Select(ToSummaryDto);
        return Ok(dtos);
    }

    // Admin & Agents - List all agents
    [HttpGet("agents")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetAgents()
    {
        var agents = await users.GetAgentsAsync();
        var dtos = agents.Select(ToSummaryDto);
        return Ok(dtos);
    }

    // Any authenticated user - Get user
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var id = JwtHelper.GetUserId(User);
        var user = await users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(ToSummaryDto(user));
    }

    // Admin & Agents - Get specific user
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(ToSummaryDto(user));
    }

    // Admin only - Update user role
    [HttpPatch("{id:int}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(int id, UpdateUserRoleDto dto)
    {
        if (!Enum.TryParse<UserRoles>(dto.Role, ignoreCase: true, out var role))
            return BadRequest(new { error = $"Invalid role '{dto.Role}'." });

        var user = await users.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.Role = role;
        await users.UpdateAsync(user);
        return Ok(ToSummaryDto(user));
    }

    // Admin only - Deactivate user
    [HttpPatch("{id:int}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.IsActive = false;
        await users.UpdateAsync(user);
        return NoContent();
    }

    // Admin only - Activate user
    [HttpPatch("{id:int}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.IsActive = true;
        await users.UpdateAsync(user);
        return NoContent();
    }

    private static UserSummaryDto ToSummaryDto(User u) =>
        new(u.Id, u.FullName, u.Email, u.Role.ToString(), u.IsActive, u.CreatedAt);
}
