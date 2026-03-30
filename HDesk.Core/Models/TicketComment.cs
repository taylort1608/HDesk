using System;

namespace HDesk.Core.Models;

public class TicketComment
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
    // Foreign keys
    public int TicketId { get; set; }
    public int AuthorId { get; set; }
 
    // Navigation
    public Ticket Ticket { get; set; } = null!;
    public User Author { get; set; } = null!;
}