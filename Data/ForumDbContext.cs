using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Fourm.Models;

namespace Fourm.Data;

public partial class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ForumThread> Threads { get; set; }
    public DbSet<ThreadReply> Replies { get; set; }
    public DbSet<PrivateMessage> PrivateMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
        });

        // Configure ForumThread entity
        modelBuilder.Entity<ForumThread>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Content).IsRequired();
            entity.Property(t => t.AuthorUsername).IsRequired().HasMaxLength(50);
            entity.Property(t => t.CategoryId).IsRequired().HasMaxLength(50);
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.HasMany(t => t.Replies)
                  .WithOne()
                  .HasForeignKey(r => r.ThreadId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ThreadReply entity
        modelBuilder.Entity<ThreadReply>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Content).IsRequired();
            entity.Property(r => r.AuthorUsername).IsRequired().HasMaxLength(50);
            entity.Property(r => r.CreatedAt).IsRequired();
            entity.Property(r => r.ThreadId).IsRequired();
        });

        // Configure PrivateMessage entity
        modelBuilder.Entity<PrivateMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.SenderUsername).IsRequired().HasMaxLength(50);
            entity.Property(m => m.RecipientUsername).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Subject).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Content).IsRequired().HasMaxLength(5000);
            entity.Property(m => m.SentAt).IsRequired();
            entity.HasIndex(m => m.SenderUsername);
            entity.HasIndex(m => m.RecipientUsername);
            entity.HasIndex(m => new { m.RecipientUsername, m.IsRead });
        });
    }
}
