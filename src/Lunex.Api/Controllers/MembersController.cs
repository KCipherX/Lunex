using Lunex.Application.Members.Services.Abstractions;
using Lunex.Domain.Enitities.Users;

using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[Route("api/members")]
public sealed class MembersController(IMemberService memberService) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var members = await memberService.GetAsync(cancellationToken);
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var member = await memberService.GetByIdAsync(id, cancellationToken);
        return member is null
            ? NotFound() 
            : Ok(member);
    }
}
