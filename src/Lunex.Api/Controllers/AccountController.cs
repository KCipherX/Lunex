using Lunex.Api.Extensions.Accounts;
using Lunex.Application.Identity.Services;
using Lunex.Contracts.Accounts.Common.Responses;
using Lunex.Contracts.Accounts.Login.Requests;
using Lunex.Contracts.Accounts.Register.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

/// <summary>
/// Controller for account authentication and registration.
/// </summary>
[Route("api/account")]
[AllowAnonymous]
[ApiController]
[Produces("application/json")]
public sealed class AccountController(IIdentityService identityService) : ApiController
{
    /// <summary>
    /// Registers a new user account (authentication only).
    /// </summary>
    /// <param name="request">The registration request containing email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registered user with JWT token.</returns>
    /// <response code="200">Returns the registered user with token.</response>
    /// <response code="400">Email already exists or invalid request.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var (email, password, name) = request.ToCommandPrimitives();
        var tokenResult = await identityService.RegisterAsync(email, password, name, cancellationToken);

        // Get the created user to populate the response with user details
        var user = await identityService.GetByEmailAsync(email, cancellationToken);
        var response = tokenResult.ToUserResponse(
            user?.Id ?? string.Empty,
            email,
            name,
            user?.ImageUrl);

        return Ok(response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authenticated user with JWT token.</returns>
    /// <response code="200">Returns the authenticated user with token.</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var (email, password) = request.ToCommandPrimitives();
        var tokenResult = await identityService.LoginAsync(email, password, cancellationToken);

        // Get the authenticated user to populate the response with user details
        var user = await identityService.GetByEmailAsync(email, cancellationToken);
        var response = tokenResult.ToUserResponse(
            user?.Id ?? string.Empty,
            email,
            user?.Name,
            user?.ImageUrl);

        return Ok(response);
    }

    /// <summary>
    /// Test endpoint to verify JSON serialization.
    /// </summary>
    /// <returns>A simple test response.</returns>
    /// <response code="200">Returns a test response.</response>
    [HttpGet("test")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Test()
    {
        var response = "Test response from AccountController";
        System.Console.WriteLine($"Test endpoint called, returning: {response}");
        var result = Ok(response);
        System.Console.WriteLine($"Ok result: {result.GetType().Name}");
        return result;
    }
}
