using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RollForBears.Api.MainModules.LoginModule.Models;

namespace roll_for_bears.Database;

public partial class RollForBearsContext : DbContext
{
    public RollForBearsContext()
    {
    }

    public RollForBearsContext(DbContextOptions<RollForBearsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("user_info", "status", new[] { "active", "pending", "suspended", "banned" });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("Account_pkey");

            entity.ToTable("account", "user_info", tb => tb.HasComment("user accounts"));

            entity.HasIndex(e => e.Email, "email_unique_constraint").IsUnique();

            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("UUID");
            entity.Property(e => e.StatusChangedAt).HasColumnType("timestamp(0) without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
