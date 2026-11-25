using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.RpgContent.Models;

namespace RollForBears.Modules.RpgContent.Database;

public class RpgContentDbContext : DbContext
{
    public RpgContentDbContext(DbContextOptions<RpgContentDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<GameSystem> GameSystems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var gameSystem = modelBuilder.Entity<GameSystem>();

        gameSystem.ToTable("game_system", "rpg_content", table =>
        {
            table.HasCheckConstraint(
                "game_system_not_blank",
                "trim(name) <> ''"
            );
        });
        
        gameSystem.HasKey(x => x.GameSystemUuid);

        gameSystem.Property(x => x.GameSystemUuid)
            .HasColumnName("g_system_uuid")
            .ValueGeneratedNever();

        gameSystem.Property(x => x.SystemKey)
            .HasColumnName("system_key")
            .HasMaxLength(64)
            .IsRequired();

        gameSystem.HasIndex(x => x.SystemKey)
            .IsUnique();

        gameSystem.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(64)
            .IsRequired();

        gameSystem.Property(x => x.GenreSet)
            .HasColumnName("genre_set")
            .HasColumnType("text[]")
            .IsRequired();

        gameSystem.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(false);

        gameSystem.Property(x => x.Settings)
            .HasColumnName("settings")
            .HasColumnType("jsonb")
            .IsRequired();
    }
}