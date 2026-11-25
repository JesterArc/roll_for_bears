using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RollForBears.Modules.RpgContent.Contracts.DTOs;

public class CreateGameSystemRequestDto
{
    [Required]
    [StringLength(64)]
    public string SystemKey { get; set; } = String.Empty;
    [Required]
    [StringLength(64)]
    public string Name { get; set; } = String.Empty;
    public string []  GenreSet { get; set; } = Array.Empty<string>();
    public string? Description { get; set; }
    public JsonElement? Settings { get; set; }
}