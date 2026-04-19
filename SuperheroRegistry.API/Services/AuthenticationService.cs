using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SuperheroRegistry.API.Interfaces;
using SuperheroRegistry.Domain.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SuperheroRegistry.API.Services;

public class AuthenticationService(UserManager<User> userManager, IConfiguration configuration) : IAuthenticationService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<(bool succeeded, string? token, string? error)> RegisterAsync(string username, string password)
    {
        var user = new User { UserName = username };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, null, errors);
        }

        var token = GenerateToken(user);
        return (true, token, null);
    }

    public async Task<(bool succeeded, string? token, string? error)> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return (false, null, "Invalid username or password.");

        var token = GenerateToken(user);
        return (true, token, null);
    }

    public string? GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId;
    }

    private string GenerateToken(User user)
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
