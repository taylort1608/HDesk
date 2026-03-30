using System;
using HDesk.Core.Models;

namespace HDesk.Core.Interfaces;

public interface ITicketCategoryRepository
{
    Task<TicketCategory?> GetByIdAsync(int id);
    Task<IEnumerable<TicketCategory>> GetAllAsync(bool includeInactive = false);
    Task<TicketCategory> CreateAsync(TicketCategory category);
    Task<TicketCategory> UpdateAsync(TicketCategory category);
    Task<bool> ExistsAsync(int id);
}
 