using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RollForBears.Modules.RpgContent.Contracts.DTOs;

public sealed class GameSystemDto
{
    public Guid GameSystemUuid  { get; set; }
    [Required]
    public string SystemKey { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string [] GenreSet { get; set; }
    public string? Description { get; set; }
    public JsonElement Settings { get; set; }
}