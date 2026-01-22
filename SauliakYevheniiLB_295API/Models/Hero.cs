using System.ComponentModel.DataAnnotations;

namespace SauliakYevheniiLB_295API.Models;

public class Hero
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = string.Empty; // tank, damage, support

    [StringLength(500)]
    public string? Portrait { get; set; } // URL zum Bild

    [StringLength(1000)]
    public string? Description { get; set; }

    // Hitpoints direkt im Hero
    public int Health { get; set; }
    public int Armor { get; set; }
    public int Shields { get; set; }

    // Navigation Property für Abilities (One-to-Many)
    public ICollection<Ability> Abilities { get; set; } = new List<Ability>();
}