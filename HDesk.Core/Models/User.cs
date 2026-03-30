using System;

namespace HDesk.Core.Models;

public enum UserRoles
{
    EndUser,
    Agent,
    Manager,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRoles Role { get; set; } = UserRoles.EndUser;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
    // Navigation
    public ICollection<Ticket> SubmittedTickets { get; set; } = [];
    public ICollection<Ticket> AssignedTickets { get; set; } = [];
    public ICollection<TicketComment> Comments { get; set; } = [];
}
