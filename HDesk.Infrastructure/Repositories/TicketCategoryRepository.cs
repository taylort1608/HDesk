using System;
using HDesk.Core.Interfaces;
using HDesk.Core.Models;
using HDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HDesk.Infrastructure.Repositories;

public class TicketCategoryRepository(AppDbContext db) : ITicketCategoryRepository
{
    public async Task<TicketCategory?> GetByIdAsync(int id) =>
        await db.TicketCategories.FindAsync(id);
 
    public async Task<IEnumerable<TicketCategory>> GetAllAsync(bool includeInactive = false) =>
        await db.TicketCategories
            .Where(c => includeInactive || c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
 
    public async Task<TicketCategory> CreateAsync(TicketCategory category)
    {
        db.TicketCategories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }
 
    public async Task<TicketCategory> UpdateAsync(TicketCategory category)
    {
        db.TicketCategories.Update(category);
        await db.SaveChangesAsync();
        return category;
    }
 
    public async Task<bool> ExistsAsync(int id) =>
        await db.TicketCategories.AnyAsync(c => c.Id == id);
}
 