using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Api.Model.Auth;
using SuperheroRegistry.API.Interfaces;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// Handles user authentication requests.
/// Delegates all authentication operations to the authentication service.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Registers a new user and returns a JWT token.
    /// </summary>
    /// <param name="registerModel">The registration details (username and password).</param>
    /// <returns>A JWT token and username on success, or validation errors on failure.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterModel registerModel)
    {
        var (succeeded, token, error) = await _authenticationService.RegisterAsync(
            registerModel.Username,
            registerModel.Password);

        if (!succeeded)
            return BadRequest(new { message = error });

        return Ok(new AuthResponse { Token = token!, Username = registerModel.Username });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginModel">The login details (username and password).</param>
    /// <returns>A JWT token and username on success, or 401 Unauthorized if credentials are invalid.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginModel loginModel)
    {
        var (succeeded, token, error) = await _authenticationService.LoginAsync(
            loginModel.Username,
            loginModel.Password);

        if (!succeeded)
            return Unauthorized(new { message = error });

        return Ok(new AuthResponse { Token = token!, Username = loginModel.Username });
    }

}
