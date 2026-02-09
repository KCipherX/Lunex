using System.Security.Cryptography;
using System.Text;

using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Contracts.Accounts.Login;
using Lunex.Contracts.Members.Requests;
using Lunex.Domain.Enitities.Users;

using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[Route("api/account")]
public sealed class AccountController(IAccountService accountService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
    {
        var user = RegisterAccountRequest(request);
        var emailExists = await accountService.EmailExistsAsync(user.Email);
        if (emailExists) 
            return BadRequest("Email exists");

        var userResponse = await accountService.RegisterAsync(user);
        return Ok(userResponse);
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
    {
        var user = await accountService.GetByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        for (var i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != user.PasswordHash[i])
                return Unauthorized();

        return Ok(user);
    }

    [NonAction]
    private static User RegisterAccountRequest(RegisterRequest request)
    {
        using var hmac = new HMACSHA512();

        User user = new()
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key
        };

        return user;
    }
}
