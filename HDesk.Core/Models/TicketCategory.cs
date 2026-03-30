using System;

namespace HDesk.Core.Models;

public class TicketCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
 
    // Navigation
    public ICollection<Ticket> Tickets { get; set; } = [];
}