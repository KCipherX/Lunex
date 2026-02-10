using System.Security.Cryptography;
using System.Text;

using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Contracts.Accounts.Login;
using Lunex.Contracts.Members.Requests;
using Lunex.Domain.Enitities.Users;

using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[Route("api/account")]
public sealed class AccountController(
    IAccountService accountService, IJwtTokenGenerator jwtTokenGenerator) 
    : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = RegisterAccountRequest(request);
        var emailExists = await accountService.EmailExistsAsync(user.Email);
        if (emailExists) 
            return BadRequest("Email exists");

        var userResponse = await accountService.RegisterAsync(user);
        return Ok(userResponse);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        User? user = await accountService.GetByEmailAsync(request.Email);
        
        if (user is null)
            return Unauthorized("Invalid email address");

        using HMACSHA512 hmac = new(user.PasswordSalt);

        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        for (var i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != user.PasswordHash[i])
                return Unauthorized();

        UserResponse response = new(
            Id: user.Id,
            Email: user.Email,
            Name: user.Name,
            ImageUrl: string.Empty,
            Token: jwtTokenGenerator.GenerateToken(user));

        return Ok(response);
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
