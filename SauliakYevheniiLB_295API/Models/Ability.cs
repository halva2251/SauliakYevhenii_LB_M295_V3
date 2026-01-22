using SauliakYevheniiLB_295API.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SauliakYevheniiLB_295API.Models;

public class Ability
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Icon { get; set; } // URL zum Icon

    // Foreign Key
    public int HeroId { get; set; }

    // Navigation Property (JsonIgnore um Circular Reference zu vermeiden)
    [JsonIgnore]
    public Hero? Hero { get; set; }
}