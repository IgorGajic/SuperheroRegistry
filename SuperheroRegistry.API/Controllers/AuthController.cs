using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SuperheroRegistry.Api.Model.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// Handles user authentication and JWT token generation.
/// Provides endpoints for user registration and login.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user and returns a JWT token.
    /// </summary>
    /// <param name="dto">The registration details (username and password).</param>
    /// <returns>A JWT token and username on success, or validation errors on failure.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterModel registerModel)
    {
        var user = new IdentityUser { UserName = registerModel.Username };
        var result = await _userManager.CreateAsync(user, registerModel.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var token = GenerateToken(user);
        return Ok(new AuthResponse { Token = token, Username = user.UserName! });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="dto">The login details (username and password).</param>
    /// <returns>A JWT token and username on success, or 401 Unauthorized if credentials are invalid.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginModel loginModel)
    {
        var user = await _userManager.FindByNameAsync(loginModel.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            return Unauthorized("Invalid username or password.");

        var token = GenerateToken(user);
        return Ok(new AuthResponse { Token = token, Username = user.UserName! });
    }

    /// <summary>
    /// Generates a JWT token for an authenticated user.
    /// Token includes user ID and username claims, valid for 8 hours.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>A signed JWT token string.</returns>
    private string GenerateToken(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),  // userId
            new Claim(ClaimTypes.Name, user.UserName!)       // username
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}