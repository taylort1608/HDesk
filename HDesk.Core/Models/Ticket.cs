using System;

namespace HDesk.Core.Models;

public enum TicketStatus
{
    Open,
    InProgress,
    OnHold,
    Resolved,
    Closed
}
 
public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical
}
 
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Low;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
 
    // Foreign keys
    public int SubmittedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public int CategoryId { get; set; }
 
    // Navigation
    public User SubmittedBy { get; set; } = null!;
    public User? AssignedTo { get; set; }
    public TicketCategory Category { get; set; } = null!;
    public ICollection<TicketComment> Comments { get; set; } = [];
}