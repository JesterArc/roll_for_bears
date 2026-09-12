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
    
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("status", new[] { "active", "pending", "suspended", "banned" });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("account_pkey");

            entity.ToTable("account", "user_info");

            entity.HasIndex(e => e.Email, "account_unique_email").IsUnique();
            
            entity.HasIndex(e => e.Username, "account_unique_username").IsUnique();

            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("UUID");
            
            entity.Property(e => e.HashWord).HasColumnName("Hash_word");
            
            entity.Property(e => e.Username).HasMaxLength(20);
            entity.Property(e => e.StatusChangedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("status");
        });
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("refresh_token_pkey");

            entity.ToTable("refresh_token", "user_info");

            entity.HasIndex(e => e.TokenHash, "refresh_token_TokenHash_key").IsUnique();

            entity.HasIndex(e => e.AccountId, "refresh_token_account_id_idx");

            entity.HasIndex(e => e.ExpiresAt, "refresh_token_expires_at_idx");

            entity.HasIndex(e => e.FamilyId, "refresh_token_family_id_idx");

            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("UUID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.RevokedAt)
                .HasColumnType("timestamp with time zone");

            entity.HasOne(d => d.Account).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("refresh_token_account_fk");

            entity.HasOne(d => d.ReplacedByToken).WithMany(p => p.InverseReplacedByToken)
                .HasForeignKey(d => d.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("refresh_token_replaced_by_token_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
