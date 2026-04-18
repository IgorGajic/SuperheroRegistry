using System.Security.Claims;

namespace SuperheroRegistry.Application.Interfaces;

public interface IAuthenticationService //ovo ide u API
{
    Task<(bool succeeded, string? token, string? error)> RegisterAsync(string username, string password);
    Task<(bool succeeded, string? token, string? error)> LoginAsync(string username, string password);
    string? GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal);
}
