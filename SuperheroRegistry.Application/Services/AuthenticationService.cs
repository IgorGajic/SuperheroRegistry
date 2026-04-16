using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperheroRegistry.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text;

namespace SuperheroRegistry.Application.Services;

/// <summary>
/// Implements authentication operations including user registration, login, and JWT token generation.
/// Encapsulates all authentication and token generation concerns away from presentation logic.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthenticationService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user and generates a JWT token.
    /// </summary>
    /// <param name="username">The username for the new account.</param>
    /// <param name="password">The password for the new account.</param>
    /// <returns>A tuple containing success status, token (if successful), and error message (if failed).</returns>
    public async Task<(bool succeeded, string? token, string? error)> RegisterAsync(string username, string password)
    {
        var user = new IdentityUser { UserName = username };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, null, errors);
        }

        var token = GenerateToken(user);
        return (true, token, null);
    }

    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="username">The username to authenticate.</param>
    /// <param name="password">The password to authenticate.</param>
    /// <returns>A tuple containing success status, token (if successful), and error message (if failed).</returns>
    public async Task<(bool succeeded, string? token, string? error)> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return (false, null, "Invalid username or password.");

        var token = GenerateToken(user);
        return (true, token, null);
    }

    /// <summary>
    /// Extracts the user ID from JWT token claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal from the authenticated user.</param>
    /// <returns>The user ID from the NameIdentifier claim.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID cannot be found in the token.</exception>
    public string GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId ?? throw new UnauthorizedAccessException("User not found in token.");
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
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
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
