using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SauliakYevheniiLB_295API.Data;
using SauliakYevheniiLB_295API.Models;

namespace SauliakYevheniiLB_295API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HeroesController : ControllerBase
{
    private readonly AppDbContext _context;

    public HeroesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/heroes
    [HttpGet]
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

    // GET: api/heroes/5
    [HttpGet("{id}")]
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

    // POST: api/heroes
    [HttpPost]
    [Authorize]
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

    // PUT: api/heroes/5
    [HttpPut("{id}")]
    [Authorize]
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

    // DELETE: api/heroes/5
    [HttpDelete("{id}")]
    [Authorize]
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