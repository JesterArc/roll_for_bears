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

            entity.HasIndex(e => e.Email, "account_email_unique").IsUnique();
            
            entity.HasIndex(e => e.Username, "account_username_unique").IsUnique();

            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("account_uuid");
            
            entity.Property(e => e.HashWord).HasColumnName("hashword");
            
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            
            entity.Property(e => e.Email).HasColumnName("email");
            
            entity.Property(e => e.StatusChangedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("status_changed_at");
            
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("status")
                .HasDefaultValueSql("'pending'");
            
            entity.Property(e => e.Username).HasMaxLength(20).HasColumnName("username");
        });
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("refresh_token_pkey");

            entity.ToTable("refresh_token", "user_info");

            entity.HasIndex(e => e.TokenHash, "refresh_token_token_hash_unique").IsUnique();

            entity.HasIndex(e => e.AccountId, "refresh_token_account_id_idx");

            entity.HasIndex(e => e.ExpiresAt, "refresh_token_expires_at_idx");

            entity.HasIndex(e => e.FamilyId, "refresh_token_family_id_idx");

            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("token_uuid");

            entity.Property(e => e.AccountId).HasColumnName("account_uuid");
            
            entity.Property(e => e.TokenHash).HasColumnName("token_hash");
            
            entity.Property(e => e.FamilyId).HasColumnName("family_uuid");
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("expires_at");

            entity.Property(e => e.RevokedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("revoked_at");

            entity.Property(e => e.ReplacedByTokenId)
                .HasColumnName("replaced_by_uuid");

            entity.HasOne(d => d.Account).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("refresh_token_account_fkey");

            entity.HasOne(d => d.ReplacedByToken).WithMany(p => p.InverseReplacedByToken)
                .HasForeignKey(d => d.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("refresh_token_replaced_by_token_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
