using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Application.DTOs;
using SuperheroRegistry.Application.Interfaces;
using System.Security.Claims;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// Controller for managing superheroes.
/// Provides endpoints for retrieving, creating, registering, retiring, and deleting heroes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase
{
    private readonly IHeroService _heroService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeroesController"/> class.
    /// </summary>
    /// <param name="heroService">The hero service for managing hero operations.</param>
    public HeroesController(IHeroService heroService)
    {
        _heroService = heroService;
    }

    /// <summary>
    /// Retrieves all registered (public) heroes without authentication.
    /// </summary>
    /// <returns>A list of registered hero DTOs.</returns>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<List<HeroDto>>> GetRegistered()
    {
        var heroes = await _heroService.GetRegisteredAsync();
        return Ok(heroes);
    }

    /// <summary>
    /// Retrieves all heroes (public and private) with authorization.
    /// </summary>
    /// <returns>A list of all hero DTOs.</returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<HeroDto>>> GetAll()
    {
        var heroes = await _heroService.GetAllAsync();
        return Ok(heroes);
    }

    /// <summary>
    /// Retrieves a specific hero by ID with authorization.
    /// </summary>
    /// <param name="id">The ID of the hero to retrieve.</param>
    /// <returns>The hero DTO if found.</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<HeroDto>> GetById(int id)
    {
        var hero = await _heroService.GetByIdAsync(id);
        return Ok(hero);
    }

    /// <summary>
    /// Creates a new hero with authorization.
    /// The hero is created in a private (unregistered) state.
    /// </summary>
    /// <param name="dto">The data transfer object containing hero creation information.</param>
    /// <returns>The created hero DTO with a 201 Created status code.</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<HeroDto>> Create(CreateHeroDto dto)
    {
        var userId = GetUserId();
        var hero = await _heroService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = hero.Id }, hero);
    }

    /// <summary>
    /// Registers a hero, making them publicly visible with authorization.
    /// </summary>
    /// <param name="id">The ID of the hero to register.</param>
    /// <returns>The updated hero DTO.</returns>
    [HttpPatch("{id}/register")]
    [Authorize]
    public async Task<ActionResult<HeroDto>> Register(int id)
    {
        var hero = await _heroService.RegisterAsync(id);
        return Ok(hero);
    }

    /// <summary>
    /// Retires a hero, removing them from active duty with authorization.
    /// </summary>
    /// <param name="id">The ID of the hero to retire.</param>
    /// <returns>The updated hero DTO.</returns>
    [HttpPatch("{id}/retire")]
    [Authorize]
    public async Task<ActionResult<HeroDto>> Retire(int id)
    {
        var hero = await _heroService.RetireAsync(id);
        return Ok(hero);
    }

    /// <summary>
    /// Deletes a hero with authorization.
    /// Only the hero's owner can delete the hero.
    /// </summary>
    /// <param name="id">The ID of the hero to delete.</param>
    /// <returns>A 204 No Content response on successful deletion.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        await _heroService.DeleteAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Retrieves the current authenticated user's ID from the JWT token claims.
    /// </summary>
    /// <returns>The user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID cannot be found in the token.</exception>
    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found in token.");
}