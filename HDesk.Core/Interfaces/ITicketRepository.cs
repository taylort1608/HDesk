using System;
using HDesk.Core.Models;

namespace HDesk.Core.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id);
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<IEnumerable<Ticket>> GetByUserAsync(int userId);
    Task<IEnumerable<Ticket>> GetAssignedToAsync(int agentUserId);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task<TicketComment> AddCommentAsync(TicketComment comment);
    Task<bool> ExistsAsync(int id);
}
 