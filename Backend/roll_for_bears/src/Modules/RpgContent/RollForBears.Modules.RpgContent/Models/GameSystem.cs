using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RollForBears.Modules.RpgContent.Models;

public class GameSystem
{
    public Guid GameSystemUuid { get; set; }
    [Required]
    [MaxLength(64)]
    public string SystemKey { get; set; } = String.Empty;
    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = String.Empty;
    public string [] GenreSet { get; set; } = Array.Empty<string>();
    public string? Description { get; set; }
    public JsonElement Settings { get; set; }
}