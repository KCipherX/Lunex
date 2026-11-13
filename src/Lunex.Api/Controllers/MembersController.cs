using System.Security.Claims;

using Lunex.Application.Members.Services.Abstractions;
using Lunex.Contracts.Members.Requests;

using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[Route("api/members")]
public sealed class MembersController(IMemberService _memberService) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var members = await _memberService.GetAsync(cancellationToken);

        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var member = await _memberService.GetByIdAsync(id, cancellationToken);

        return member is null
            ? NotFound()
            : Ok(member);
    }

    [HttpGet("{id}/photos")]
    public async Task<IActionResult> GetPhotosById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var memberPhotos = await _memberService.GetPhotosAsync(id, cancellationToken);

        return memberPhotos is null
            ? NotFound()
            : Ok(memberPhotos);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] MemberUpdateRequest request, CancellationToken cancellationToken)
    {
        var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(memberId))
        {
            return BadRequest("Oops - no id found in token");
        }

        var member = await _memberService.GetByIdAsync(memberId, cancellationToken);

        if (member is null)
        {
            return NotFound();
        }

        member.Name = request.DisplayName;
        member.Description = request.Description;
        member.City = request.City;
        member.Country = request.Country;

        await _memberService.UpdateAsync(member, cancellationToken);

        return Ok(member);
    }
}
