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
public class TicketsController(ITicketRepository tickets, ITicketCategoryRepository categories, IUserRepository users) : ControllerBase
{
    // Admin & Agent - List all tickets
    // EndUser - List user tickets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var callerId = JwtHelper.GetUserId(User);
        var role = JwtHelper.GetUserRole(User);

        var list = role is "Admin" or "Agent"
            ? await tickets.GetAllAsync()
            : await tickets.GetByUserAsync(callerId);

        return Ok(list.Select(ToSummaryDto));
    }

    // Admin & Agent - Get tickets assigned to user
    [HttpGet("assigned")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetAssigned()
    {
        var callerId = JwtHelper.GetUserId(User);
        var list = await tickets.GetAssignedToAsync(callerId);

        return Ok(list.Select(ToSummaryDto));
    }

    // Get specific ticket by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound();

        // Lets EndUsers view their own ticket only
        var callerId = JwtHelper.GetUserId(User);
        var role = JwtHelper.GetUserRole(User);
        if (role == "EndUser" && ticket.SubmittedByUserId != callerId)
            return Forbid();

        return Ok(ToDetailDto(ticket, role));
    }

    // Create ticket
    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketDto createTicketDto)
    {
        if (!Enum.TryParse<TicketPriority>(createTicketDto.Priority, ignoreCase: true, out var priority))
            return BadRequest(new { error = $"Invalid priority '{createTicketDto.Priority}" });

        if (!await categories.ExistsAsync(createTicketDto.CategoryId))
            return BadRequest(new { error = "Category not found" });

        var callerId = JwtHelper.GetUserId(User);
        var ticket = new Ticket
        {
            Title = createTicketDto.Title,
            Description = createTicketDto.Description,
            Priority = priority,
            CategoryId = createTicketDto.CategoryId,
            SubmittedByUserId = callerId
        };

        var created = await tickets.CreateAsync(ticket);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDetailDto(created, JwtHelper.GetUserRole(User)));
    }

    // Admin & Agent only - Update ticket
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Update(int id, UpdateTicketDto updateTicketDto)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound();

        if (updateTicketDto.Status is not null)
        {
            if (!Enum.TryParse<TicketStatus>(updateTicketDto.Status, ignoreCase: true, out var status))
                return BadRequest(new { error = $"Invalid status '{updateTicketDto.Status}'." });
        }

        if (updateTicketDto.Priority is not null)
        {
            if (!Enum.TryParse<TicketPriority>(updateTicketDto.Priority, ignoreCase: true, out var priority))
                return BadRequest(new { error = $"Invalid priority '{updateTicketDto.Priority}'." });
            ticket.Priority = priority;
        }

        if (updateTicketDto.Title is not null) ticket.Title = updateTicketDto.Title;
        if (updateTicketDto.Description is not null) ticket.Description = updateTicketDto.Description;

        if (updateTicketDto.CategoryId is not null)
        {
            if (!await categories.ExistsAsync(updateTicketDto.CategoryId.Value))
                return BadRequest(new { error = "Category not found." });
            ticket.CategoryId = updateTicketDto.CategoryId.Value;
        }

        var updated = await tickets.UpdateAsync(ticket);

        return Ok(ToDetailDto(updated, JwtHelper.GetUserRole(User)));
    }

    // Admin & Agent only - Assign ticket
    [HttpPatch("{id:int}/assign")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Assign(int id, AssignTicketDto assignTicketDto)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound();

        if (assignTicketDto.AgentUserId is not null)
        {
            var agent = await users.GetByIdAsync(assignTicketDto.AgentUserId.Value);
            if (agent is null || agent.Role == UserRoles.EndUser)
                return BadRequest(new { error = "Assigned user must be an Agent or Admin." });
        }

        ticket.AssignedToUserId = assignTicketDto.AgentUserId;
        if (ticket.Status == TicketStatus.Open && assignTicketDto.AgentUserId is not null)
            ticket.Status = TicketStatus.InProgress;

        var updated = await tickets.UpdateAsync(ticket);
        
        return Ok(ToDetailDto(updated, JwtHelper.GetUserRole(User)));
    }

    // Add comment to ticket
    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, AddCommentDto addCommentDto)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound();

        var callerId = JwtHelper.GetUserId(User);
        var role = JwtHelper.GetUserRole(User);

        // Prevents EndUsers from adding internal comments/comments on other's tickets
        if (role == "EndUser")
        {
            if (ticket.SubmittedByUserId != callerId) return Forbid();
            if (addCommentDto.IsInternal) return Forbid();
        }

        var comment = new TicketComment
        {
            TicketId = id,
            AuthorId = callerId,
            Body = addCommentDto.Body,
            IsInternal = addCommentDto.IsInternal
        };

        var created = await tickets.AddCommentAsync(comment);

        return Ok(new CommentDto(
            created.Id,
            created.Body,
            created.IsInternal,
            created.Author.FullName,
            created.CreatedAt));
    }

    private static TicketSummaryDto ToSummaryDto(Ticket t) => new(
        t.Id,
        t.Title,
        t.Status.ToString(),
        t.Priority.ToString(),
        t.Category.Name,
        t.SubmittedBy.FullName,
        t.AssignedTo?.FullName,
        t.CreatedAt,
        t.UpdatedAt);

    private static TicketDetailDto ToDetailDto(Ticket t, string callerRole) => new(
        t.Id,
        t.Title,
        t.Description,
        t.Status.ToString(),
        t.Priority.ToString(),
        t.CategoryId,
        t.Category.Name,
        t.SubmittedByUserId,
        t.SubmittedBy.FullName,
        t.AssignedToUserId,
        t.AssignedTo?.FullName,
        t.CreatedAt,
        t.UpdatedAt,
        t.ResolvedAt,
        t.Comments
            .Where(c => !c.IsInternal || callerRole != "EndUser")
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(c.Id, c.Body, c.IsInternal, c.Author.FullName, c.CreatedAt)));
}
