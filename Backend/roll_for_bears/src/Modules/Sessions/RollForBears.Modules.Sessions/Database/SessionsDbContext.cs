using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Models;

namespace RollForBears.Modules.Sessions.Database;

public partial class SessionsDbContext : DbContext
{
    public SessionsDbContext(DbContextOptions<SessionsDbContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<SessionPlayer> SessionPlayers { get; set; }
    public virtual DbSet<JoinRequest> JoinRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<GameType>("gameplay", "game_type");
        modelBuilder.HasPostgresEnum<PlayerRole>("gameplay", "player_role");
        
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("session_details_pkey");

            entity.ToTable("session_details", "gameplay");
            
            entity.Property(e => e.Uuid)
                .ValueGeneratedNever()
                .HasColumnName("session_uuid")
                .IsRequired();
            
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("session_name")
                .IsRequired();
            
            entity.Property(e => e.Tagline)
                .HasMaxLength(50)
                .IsRequired(false)
                .HasColumnName("tagline")
                .IsRequired();
            
            entity.Property(e => e.IsGeneratingNotes)
                .HasColumnName("generating_notes")
                .IsRequired();
            
            entity.Property(e => e.GameSystem)
                .HasColumnName("game_system")
                .IsRequired();
            
            entity.Property(e => e.GameType)
                .HasColumnName("game_type")
                .HasColumnType("game_type")
                .IsRequired();
            
            entity.Property(e => e.MinimalPlayers)
                .HasColumnName("min_players")
                .IsRequired();
            
            entity.Property(e => e.MaximalPlayers)
                .HasColumnName("max_players")
                .IsRequired();
            
            entity.Property(e => e.GameLanguage)
                .HasColumnName("game_language")
                .IsRequired();
            
            entity.Property(e => e.OwnerUuid)
                .HasColumnName("owner_uuid")
                .IsRequired();
            
            entity.Property(e => e.SessionDescription)
                .HasColumnName("session_description")
                .IsRequired(false);
            
            entity.Property(e => e.NextSessionDate)
                .HasColumnName("next_session_date")
                .HasColumnType("timestamp(0) with time zone")
                .IsRequired(false);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp(0) with time zone")
                .IsRequired();
            
            entity.Property(e => e.LastUpdatedAt)
                .HasColumnName("last_updated_at")
                .HasColumnType("timestamp(0) with time zone")
                .IsRequired();
        });

        modelBuilder.Entity<SessionPlayer>(entity =>
        {
            entity.HasKey(e => e.ConnectionId).HasName("session_players_pkey");

            entity.ToTable("session_players", "gameplay");

            entity.HasIndex(e => new {e.AccountId, e.SessionId}, "account_session_session_players_unique").IsUnique();
            
            entity.Property(e => e.ConnectionId)
                .ValueGeneratedNever()
                .HasColumnName("connection_uuid")
                .IsRequired();
            
            entity.Property(e => e.AccountId)
                .HasColumnName("account_uuid")
                .IsRequired();
            
            entity.Property(e => e.SessionId)
                .HasColumnName("session_uuid")
                .IsRequired();
            
            entity.Property(e => e.Role)
                .HasColumnName("player_role")
                .HasColumnType("player_role")
                .IsRequired();
            
            entity.HasOne(e => e.Session).WithMany(e => e.Players)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_uuid_fkey");
        });

        modelBuilder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(e => e.ConnectionUuid).HasName("join_requests_pkey");

            entity.ToTable("join_requests", "gameplay");

            entity.HasIndex(e => new { e.AccountUuid, e.SessionUuid }, "account_session_join_request_unique")
                .IsUnique();

            entity.Property(e => e.ConnectionUuid)
                .ValueGeneratedNever()
                .HasColumnName("connection_uuid")
                .IsRequired();
            
            entity.Property(e => e.AccountUuid)
                .HasColumnName("account_uuid")
                .IsRequired();

            entity.Property(e => e.SessionUuid)
                .HasColumnName("session_uuid")
                .IsRequired();

            entity.Property(e => e.PlayerRole)
                .HasColumnName("player_role")
                .HasColumnType("player_role")
                .IsRequired();

            entity.HasOne(e => e.Session).WithMany(e => e.JoinRequests)
                .HasForeignKey(e => e.SessionUuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_uuid_fkey");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}