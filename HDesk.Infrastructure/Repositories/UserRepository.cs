using System;
using HDesk.Core.Interfaces;
using HDesk.Core.Models;
using HDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HDesk.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id) =>
        await db.Users.FindAsync(id);
 
    public async Task<User?> GetByEmailAsync(string email) =>
        await db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
 
    public async Task<IEnumerable<User>> GetAllAsync() =>
        await db.Users.OrderBy(u => u.FullName).ToListAsync();
 
    public async Task<IEnumerable<User>> GetAgentsAsync() =>
        await db.Users
            .Where(u => u.Role == UserRoles.Agent && u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();
 
    public async Task<User> CreateAsync(User user)
    {
        user.Email = user.Email.ToLower();
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }
 
    public async Task<User> UpdateAsync(User user)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync();
        return user;
    }
 
    public async Task<bool> ExistsAsync(int id) =>
        await db.Users.AnyAsync(u => u.Id == id);
 
    public async Task<bool> EmailExistsAsync(string email) =>
        await db.Users.AnyAsync(u => u.Email == email.ToLower());
}
 