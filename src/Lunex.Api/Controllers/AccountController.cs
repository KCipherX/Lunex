using System.Security.Cryptography;
using System.Text;

using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Contracts.Members.Requests;
using Lunex.Domain.Enitities.Users;

using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[Route("api/account")]
public sealed class AccountController(IAccountService accountService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
    {
        var hmac = new HMACSHA512();
        var user = RegisterAccountDto(request, hmac);
        var userResponse = await accountService.RegisterAsync(user);

        return Ok(userResponse);
    }

    [NonAction]
    private static User RegisterAccountDto(RegisterRequest request, HMACSHA512 hmac) => new()
    {
        Name = request.Name,
        Email = request.Email,
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
        PasswordSalt = hmac.Key
    };
}
