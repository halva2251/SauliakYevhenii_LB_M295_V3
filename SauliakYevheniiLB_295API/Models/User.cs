using System.ComponentModel.DataAnnotations;

namespace SauliakYevheniiLB_295API.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gespeicherter Refresh Token für Token-Validierung
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Ablaufzeit des Refresh Tokens
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
