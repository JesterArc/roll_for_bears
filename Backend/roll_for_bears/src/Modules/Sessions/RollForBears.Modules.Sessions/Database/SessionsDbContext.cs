using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Sessions.Models;

namespace RollForBears.Modules.Sessions.Database;

public partial class SessionsDbContext : DbContext
{
    public SessionsDbContext(DbContextOptions<SessionsDbContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<SessionPlayer> SessionPlayers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("user_info", "game_type", ["campaign", "oneshot"]);
        modelBuilder.HasPostgresEnum("user_info", "player_role", ["gamemaster", "player"]);
        
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("session_details_pkey");

            entity.ToTable("session_details", "sessions_info");
            
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
                .HasColumnType("date")
                .IsRequired(false);
        });

        modelBuilder.Entity<SessionPlayer>(entity =>
        {
            entity.HasKey(e => e.ConnectionId).HasName("session_players_pkey");

            entity.ToTable("session_players", "sessions_info");

            entity.HasIndex(e => new {e.AccountId, e.SessionId}, "account_session_unique").IsUnique();
            
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
        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}