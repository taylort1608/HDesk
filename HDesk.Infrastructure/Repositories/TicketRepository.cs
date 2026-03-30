using System;
using HDesk.Core.Interfaces;
using HDesk.Core.Models;
using HDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HDesk.Infrastructure.Repositories;

public class TicketRepository(AppDbContext db) : ITicketRepository
{
    private IQueryable<Ticket> WithIncludes() =>
        db.Tickets
          .Include(t => t.SubmittedBy)
          .Include(t => t.AssignedTo)
          .Include(t => t.Category)
          .Include(t => t.Comments)
              .ThenInclude(c => c.Author);
 
    public async Task<Ticket?> GetByIdAsync(int id) =>
        await WithIncludes().FirstOrDefaultAsync(t => t.Id == id);
 
    public async Task<IEnumerable<Ticket>> GetAllAsync() =>
        await WithIncludes()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
 
    public async Task<IEnumerable<Ticket>> GetByUserAsync(int userId) =>
        await WithIncludes()
            .Where(t => t.SubmittedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
 
    public async Task<IEnumerable<Ticket>> GetAssignedToAsync(int agentUserId) =>
        await WithIncludes()
            .Where(t => t.AssignedToUserId == agentUserId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
 
    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();
        return (await GetByIdAsync(ticket.Id))!;
    }
 
    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        ticket.UpdatedAt = DateTime.UtcNow;
        db.Tickets.Update(ticket);
        await db.SaveChangesAsync();
        return (await GetByIdAsync(ticket.Id))!;
    }
 
    public async Task<TicketComment> AddCommentAsync(TicketComment comment)
    {
        db.TicketComments.Add(comment);
        await db.SaveChangesAsync();
        await db.Entry(comment).Reference(c => c.Author).LoadAsync();
        return comment;
    }
 
    public async Task<bool> ExistsAsync(int id) =>
        await db.Tickets.AnyAsync(t => t.Id == id);
}