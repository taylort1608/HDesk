using System;
using HDesk.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HDesk.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // List of DB tables
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    // Configure the model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        // Ticket
        modelBuilder.Entity<Ticket>(e =>
        {
            e.Property(t => t.Status).HasConversion<string>();
            e.Property(t => t.Priority).HasConversion<string>();
 
            e.HasOne(t => t.SubmittedBy)
             .WithMany(u => u.SubmittedTickets)
             .HasForeignKey(t => t.SubmittedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
 
            e.HasOne(t => t.AssignedTo)
             .WithMany(u => u.AssignedTickets)
             .HasForeignKey(t => t.AssignedToUserId)
             .OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
 
            e.HasOne(t => t.Category)
             .WithMany(c => c.Tickets)
             .HasForeignKey(t => t.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });
 
        // Ticket Comment
        modelBuilder.Entity<TicketComment>(e =>
        {
            e.HasOne(c => c.Ticket)
             .WithMany(t => t.Comments)
             .HasForeignKey(c => c.TicketId)
             .OnDelete(DeleteBehavior.Cascade);
 
            e.HasOne(c => c.Author)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.AuthorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed default categories
        modelBuilder.Entity<TicketCategory>().HasData(
            new TicketCategory { Id = 1, Name = "Technical Support", Description = "Hardware, software, and connectivity issues" },
            new TicketCategory { Id = 2, Name = "Billing", Description = "Payment and invoice enquiries" },
            new TicketCategory { Id = 3, Name = "General Enquiry", Description = "General questions and information requests" }
        );
    }
}
