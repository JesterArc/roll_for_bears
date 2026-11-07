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
    
    public virtual DbSet<Friend> Friends { get; set; }
    
    //public virtual DbSet

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("users", "status", new[] { "active", "pending", "suspended", "banned" });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("account_pkey");
            
            entity.ToTable("account", "users");

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
                .HasColumnType("users.status")
                .HasDefaultValueSql("'pending'");
            
            entity.Property(e => e.Username).HasMaxLength(20).HasColumnName("username");
        });
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("refresh_token_pkey");

            entity.ToTable("refresh_token", "users");

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

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.FriendshipUuid).HasName("friends_pkey");

            entity.ToTable("friends", "users");

            entity.ToTable(t =>
                t.HasCheckConstraint("different_users_check", "user_uuid <> friend_uuid"));

            entity.HasIndex(e => new { user_uuid=e.UserUuid, friend_uuid=e.FriendUuid }, "friends_user_friend_unique")
                .HasMethod("(greatest(user_uuid, friend_uuid), least(user_uuid, friend_uuid))")
                .IsUnique();

            entity.Property(e => e.FriendshipUuid)
                .ValueGeneratedNever()
                .HasColumnName("friendship_uuid");

            entity.Property(e => e.UserUuid)
                .HasColumnName("user_uuid");

            entity.Property(e => e.FriendUuid)
                .HasColumnName("friend_uuid");

            entity.Property(e => e.FriendshipMutual)
                .HasColumnName("friendship_mutual")
                .HasDefaultValueSql("false");

            entity.HasOne(e => e.User).WithMany(p => p.Friends)
                .HasForeignKey(e => e.UserUuid)
                .HasConstraintName("account_user_uuid_fkey");

            entity.HasOne(e => e.UsersFriend).WithMany(p => p.FriendOf)
                .HasForeignKey(e => e.FriendUuid)
                .HasConstraintName("account_friend_uuid_fkey");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
