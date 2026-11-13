using Lunex.Api.Extensions.Utilities;
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

        return member is null ? NotFound() : Ok(member);
    }

    [HttpGet("{id}/photos")]
    public async Task<IActionResult> GetPhotosById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var memberPhotos = await _memberService.GetPhotosAsync(id, cancellationToken);

        return memberPhotos is null ? NotFound() : Ok(memberPhotos);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] MemberUpdateRequest request, CancellationToken cancellationToken)
    {
        var memberId = User.GetRequiredUserId();

        var member = await _memberService.UpdateAsync(memberId, request, cancellationToken);

        return member is null ? NotFound() : Ok(member);
    }

    [HttpPost("photos"), RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UploadPhoto(IFormFile file, CancellationToken cancellationToken)
    {
        var memberId = User.GetRequiredUserId();

        if (file is null || file.Length <= 0) return BadRequest("No file uploaded");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension)) return BadRequest("Invalid file type. Allowed: jpg, jpeg, png, gif, webp");

        using var stream = file.OpenReadStream();

        var photo = await _memberService.UploadPhotoAsync(
            memberId,
            stream,
            file.FileName,
            cancellationToken);

        return Ok(photo);
    }

    [HttpDelete("photos/{photoId}")]
    public async Task<IActionResult> DeletePhoto([FromRoute] string photoId, CancellationToken cancellationToken)
    {
        var memberId = User.GetRequiredUserId();

        var result = await _memberService.DeletePhotoAsync(
            memberId,
            photoId,
            cancellationToken);

        return result ? NoContent() : NotFound();
    }
}
