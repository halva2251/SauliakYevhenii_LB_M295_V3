using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SauliakYevheniiLB_295API.Data;
using SauliakYevheniiLB_295API.Models;

namespace SauliakYevheniiLB_295API.Controllers;

/// <summary>
/// Controller für die Verwaltung von Overwatch Heroes
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class HeroesController : ControllerBase
{
    private readonly AppDbContext _context;

    public HeroesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gibt alle Heroes zurück, optional gefiltert nach Rolle oder Name
    /// </summary>
    /// <param name="role">Filtert nach Rolle (tank, damage, support)</param>
    /// <param name="name">Filtert nach Name (enthält)</param>
    /// <returns>Liste von Heroes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Hero>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes(
        [FromQuery] string? role,
        [FromQuery] string? name)
    {
        var query = _context.Heroes.Include(h => h.Abilities).AsQueryable();

        // Filter by role
        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(h => h.Role.ToLower() == role.ToLower());
        }

        // Filter by name (contains)
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(h => h.Name.ToLower().Contains(name.ToLower()));
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Gibt einen einzelnen Hero anhand seiner ID zurück
    /// </summary>
    /// <param name="id">Die ID des Heroes</param>
    /// <returns>Der gefundene Hero</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Hero), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Hero>> GetHero(int id)
    {
        var hero = await _context.Heroes
            .Include(h => h.Abilities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hero == null)
        {
            return NotFound(new { message = $"Hero mit ID {id} nicht gefunden" });
        }

        return hero;
    }

    /// <summary>
    /// Erstellt einen neuen Hero (Authentifizierung erforderlich)
    /// </summary>
    /// <param name="hero">Der zu erstellende Hero</param>
    /// <returns>Der erstellte Hero</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Hero), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Hero>> CreateHero(Hero hero)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Heroes.Add(hero);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHero), new { id = hero.Id }, hero);
    }

    /// <summary>
    /// Aktualisiert einen bestehenden Hero (Authentifizierung erforderlich)
    /// </summary>
    /// <param name="id">Die ID des zu aktualisierenden Heroes</param>
    /// <param name="hero">Die aktualisierten Hero-Daten</param>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHero(int id, Hero hero)
    {
        if (id != hero.Id)
        {
            return BadRequest(new { message = "ID stimmt nicht überein" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Delete existing abilities and add new ones
        var existingHero = await _context.Heroes
            .Include(h => h.Abilities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (existingHero == null)
        {
            return NotFound(new { message = $"Hero mit ID {id} nicht gefunden" });
        }

        // Update properties
        existingHero.Name = hero.Name;
        existingHero.Role = hero.Role;
        existingHero.Portrait = hero.Portrait;
        existingHero.Description = hero.Description;
        existingHero.Health = hero.Health;
        existingHero.Armor = hero.Armor;
        existingHero.Shields = hero.Shields;

        // Update abilities
        _context.Abilities.RemoveRange(existingHero.Abilities);
        existingHero.Abilities = hero.Abilities;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HeroExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Löscht einen Hero anhand seiner ID (Authentifizierung erforderlich)
    /// </summary>
    /// <param name="id">Die ID des zu löschenden Heroes</param>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHero(int id)
    {
        var hero = await _context.Heroes.FindAsync(id);

        if (hero == null)
        {
            return NotFound(new { message = $"Hero mit ID {id} nicht gefunden" });
        }

        _context.Heroes.Remove(hero);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HeroExists(int id)
    {
        return _context.Heroes.Any(e => e.Id == id);
    }
}
