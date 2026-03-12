using System.Security.Cryptography;
using System.Text;

using Lunex.Api.Extensions.Accounts;
using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Contracts.Accounts.Login;
using Lunex.Contracts.Members.Requests;

using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[Route("api/account")]
public sealed class AccountController(
    IAccountService accountService, 
    IJwtTokenGenerator jwtTokenGenerator)
    : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = request.ToDto(hmac: new());
        var emailExists = await accountService.EmailExistsAsync(
            user.Email, 
            cancellationToken);
        
        if (emailExists)
            return BadRequest("Email exists");

        var userResponse = await accountService.RegisterAsync(
            user, 
            cancellationToken);

        var response = userResponse?.ToDto(jwtTokenGenerator);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await accountService.GetByEmailAsync(
            email: request.Email,
            cancellationToken: cancellationToken);

        if (user is null)        
            return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);        
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
        
        for (var i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != user.PasswordHash[i])
                return Unauthorized("Invalid password");

        var response = user.ToDto(jwtTokenGenerator);
        return Ok(response);
    }
}
