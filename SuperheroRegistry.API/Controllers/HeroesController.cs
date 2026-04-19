using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Api.Model.RequestModels;
using SuperheroRegistry.Api.Model.ResponseModels;
using SuperheroRegistry.API.Interfaces;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Model;

namespace SuperheroRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase
{
    private readonly IHeroService _heroService;
    private readonly IAuthenticationService _authenticationService;
    public HeroesController(IHeroService heroService, IAuthenticationService authenticationService)
    {
        _heroService = heroService;
        _authenticationService = authenticationService;
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<List<HeroResponse>>> GetRegistered()
    {
        var heroes = await _heroService.GetRegisteredAsync();
        var heroResponses = heroes.Select(hero => new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            Status = hero.Status.ToString(),
            OriginStory = hero.OriginStory,
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        }).ToList();
        return Ok(heroResponses);
    }

    [HttpGet("exists/{codename}")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> CodenameExists(string codename)
    {
        var exists = await _heroService.CodenameExistsAsync(codename);
        return Ok(exists);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<HeroResponse>>> GetAll()
    {
        var heroes = await _heroService.GetAllAsync();
        var heroResponses = heroes.Select(hero => new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Status = hero.Status.ToString(),
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        }).ToList();
        return Ok(heroResponses);
    }

    /// <summary>
    /// Retrieves all heroes created by the logged-in user
    /// </summary>
    [HttpGet("my-heroes")]
    [Authorize]
    public async Task<ActionResult<List<HeroResponse>>> GetMyHeroes()
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        if(userId == null)
            return Unauthorized("Invalid user token.");

        var heroes = await _heroService.GetByUserIdAsync(userId);
        var heroResponses = heroes.Select(hero => new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Status = hero.Status.ToString(),
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        }).ToList();
        return Ok(heroResponses);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<HeroResponse>> GetById(int id)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        if(userId == null)
            return Unauthorized("Invalid user token.");

        var hero = await _heroService.GetByIdAsync(id);

        if(hero == null)
            return NotFound($"Hero with ID {id} not found.");

        if (hero.UserId != userId)
            return StatusCode(403, "You don't have permission to view this hero.");

        var heroResponse = new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Status = hero.Status.ToString(),
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        };
        return Ok(heroResponse);
    }

    [HttpPut] 
    [Authorize]
    public async Task<ActionResult<HeroResponse>> Create(CreateHeroModel createHeroModel)
    {
        if (!Enum.TryParse<Race>(createHeroModel.Race, ignoreCase: true, out var race))
            return BadRequest($"Invalid race: {createHeroModel.Race}");

        if (!Enum.TryParse<Alignment>(createHeroModel.Alignment, ignoreCase: true, out var alignment))
            return BadRequest($"Invalid alignment: {createHeroModel.Alignment}");

        var createHero = new CreateHero
        {
            UserId = createHeroModel.UserId,
            Codename = createHeroModel.Codename,
            OriginStory = createHeroModel.OriginStory,
            Race = race,
            Alignment = alignment
        };

        var hero = await _heroService.CreateAsync(createHero);

        var heroResponse = new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Status = hero.Status.ToString(),
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        };
        return CreatedAtAction(nameof(GetById), new { id = hero.Id }, heroResponse);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<HeroResponse>> Update(UpdateHeroModel updateHeroModel)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        if(userId == null)
            return Unauthorized("Invalid user token.");

        if (userId != updateHeroModel.UserId)
            return Forbid("You don't have permission to update this hero.");

        if (!Enum.TryParse<Race>(updateHeroModel.Race, ignoreCase: true, out var race))
            return BadRequest($"Invalid race: {updateHeroModel.Race}");

        if (!Enum.TryParse<Alignment>(updateHeroModel.Alignment, ignoreCase: true, out var alignment))
            return BadRequest($"Invalid alignment: {updateHeroModel.Alignment}");
       
        var hero = await _heroService.UpdateAsync(new UpdateHero
        {
            Id = updateHeroModel.Id,
            UserId = updateHeroModel.UserId,
            Codename = updateHeroModel.Codename,
            OriginStory = updateHeroModel.OriginStory,
            Race = race,
            Alignment = alignment
        });

        var heroResponse = new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Status = hero.Status.ToString(),
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        };
        return Ok(heroResponse);
    }

    [HttpPost("{id}/register")]
    [Authorize]
    public async Task<ActionResult<HeroResponse>> Register(int id)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);

        if(userId == null)
            return Unauthorized("Invalid user token.");

        var hero = await _heroService.GetByIdAsync(id);

        if(hero == null)
            return NotFound($"Hero with ID {id} not found.");

        if(hero.UserId != userId)
            return Forbid("You don't have permission to register this hero.");

        hero = await _heroService.RegisterAsync(hero);
        var heroResponse = new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Status = hero.Status.ToString(),
            Race = hero.Race.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        };
        return Ok(heroResponse);
    }

    [HttpPost("{id}/retire")] 
    [Authorize]
    public async Task<ActionResult<HeroResponse>> Retire(int id)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);

        if(userId == null)
            return Unauthorized("Invalid user token.");

        var hero = await _heroService.GetByIdAsync(id);

        if(hero.UserId != userId)
            return Forbid("You don't have permission to retire this hero.");

        hero = await _heroService.RetireAsync(hero);

        var heroResponse = new HeroResponse
        {
            Id = hero.Id,
            UserId = hero.UserId,
            Codename = hero.Codename,
            OriginStory = hero.OriginStory,
            Race = hero.Race.ToString(),
            Status = hero.Status.ToString(),
            Alignment = hero.Alignment.ToString(),
            Powers = hero.Powers
        };
        return Ok(heroResponse);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        if(userId == null)
            return Unauthorized("Invalid user token.");

        var hero = await _heroService.GetByIdAsync(id);
       
        if(hero == null)
            return NotFound($"Hero with ID {id} not found.");

        if (hero.UserId != userId)
            return Forbid("You can only delete your own heroes.");

        await _heroService.DeleteAsync(hero);
        return NoContent();
    }
}