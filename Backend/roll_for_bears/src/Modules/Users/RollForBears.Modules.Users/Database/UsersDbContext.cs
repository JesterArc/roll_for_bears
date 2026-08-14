using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Users.Models;

namespace RollForBears.Modules.Users.Database;

public partial class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("status", new[] { "active", "pending", "suspended", "banned" });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("account_pkey");

            entity.ToTable("account", "user_info");

            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("UUID");
            entity.Property(e => e.StatusChangedAt).HasColumnType("timestamp(0) without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
