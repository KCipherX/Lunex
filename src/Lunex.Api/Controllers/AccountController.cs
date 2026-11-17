using System.Security.Cryptography;
using System.Text;

using Lunex.Api.Extensions.Accounts;
using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Application.Common.Services.Abstractions;
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
public sealed class AccountController(IAccountService accountService, IJwtTokenGenerator jwtTokenGenerator) : ApiController
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registered user with JWT token.</returns>
    /// <response code="200">Returns the registered user with token.</response>
    /// <response code="400">Email already exists or invalid request.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = request.FromDto();

        var emailExists = await accountService.EmailExistsAsync(
            user.Email,
            cancellationToken);

        if (emailExists) return BadRequest("Email exists");

        var userResponse = await accountService.RegisterAsync(
            user,
            cancellationToken);

        var response = userResponse?.ToDto(jwtTokenGenerator);

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
        var user = await accountService.GetByEmailAsync(
            email: request.Email,
            cancellationToken: cancellationToken);

        if (user is null) return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        for (var i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");

        var response = user.ToDto(jwtTokenGenerator);

        return Ok(response);
    }
}
