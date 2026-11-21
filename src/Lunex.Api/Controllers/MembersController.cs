using Lunex.Api.Extensions.Members;
using Lunex.Api.Extensions.Utilities;
using Lunex.Application.Member.Services;
using Lunex.Contracts.Members.Requests;
using Lunex.Contracts.Members.Responses;
using Lunex.Domain.Common.Pagination;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

/// <summary>
/// Controller for managing member profiles and photos.
/// </summary>
[Route("api/members")]
[ApiController]
[Produces("application/json")]
public sealed class MembersController(IMemberService _memberService) : ApiController
{
    /// <summary>
    /// Gets a paginated list of members with optional filtering.
    /// </summary>
    /// <param name="memberParams">Pagination and filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of members.</returns>
    /// <response code="200">Returns the paginated list of members.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<MemberResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] MemberParams memberParams, CancellationToken cancellationToken)
    {
        var paginatedMembers = await _memberService.GetAsync(memberParams, cancellationToken);
        return Ok(paginatedMembers.ToPaginatedResponse());
    }

    /// <summary>
    /// Gets a member by their unique identifier.
    /// </summary>
    /// <param name="id">The member's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The member if found.</returns>
    /// <response code="200">Returns the member.</response>
    /// <response code="404">Member not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var member = await _memberService.GetByIdAsync(id, cancellationToken);
        return member is null ? NotFound() : Ok(member.ToMemberResponse());
    }

    /// <summary>
    /// Gets the current authenticated user's member profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The member profile if found.</returns>
    /// <response code="200">Returns the member profile.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Member profile not found.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var member = await _memberService.GetByUserIdAsync(userId, cancellationToken);
        return member is null ? NotFound() : Ok(member.ToMemberResponse());
    }

    /// <summary>
    /// Creates a member profile for the authenticated user.
    /// </summary>
    /// <param name="request">The member profile creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created member profile.</returns>
    /// <response code="201">Returns the created member profile.</response>
    /// <response code="400">Invalid request or profile already exists.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost("profile")]
    [Authorize]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProfile([FromBody] MemberCreateRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var primitives = request.ToCommandPrimitives(userId);
        var member = await _memberService.CreateProfileAsync(
            primitives.UserId,
            primitives.Name,
            primitives.Gender,
            primitives.DateOfBirth,
            primitives.City,
            primitives.Country,
            primitives.Description,
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = member.Id }, member.ToMemberResponse());
    }

    /// <summary>
    /// Updates the authenticated user's member profile.
    /// </summary>
    /// <param name="request">The member profile update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated member profile.</returns>
    /// <response code="200">Returns the updated member profile.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Member profile not found.</response>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] MemberUpdateRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var member = await _memberService.GetByUserIdAsync(userId, cancellationToken);
        if (member is null) return NotFound();

        var primitives = request.ToCommandPrimitives(member.Id);
        var updatedMember = await _memberService.UpdateProfileAsync(
            primitives.MemberId,
            primitives.Name,
            primitives.Gender,
            primitives.DateOfBirth,
            primitives.City,
            primitives.Country,
            primitives.Description,
            primitives.ImageUrl,
            cancellationToken);

        return updatedMember is null ? NotFound() : Ok(updatedMember.ToMemberResponse());
    }

    /// <summary>
    /// Gets all photos for a specific member.
    /// </summary>
    /// <param name="id">The member's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of member photos.</returns>
    /// <response code="200">Returns the list of photos.</response>
    /// <response code="404">Member not found.</response>
    [HttpGet("{id}/photos")]
    [ProducesResponseType(typeof(IReadOnlyList<PhotoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhotosById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var memberPhotos = await _memberService.GetPhotosAsync(id, cancellationToken);
        return Ok(memberPhotos.Select(p => p.ToPhotoResponse()).ToList());
    }

    /// <summary>
    /// Uploads a photo for the authenticated member.
    /// </summary>
    /// <param name="file">The photo file to upload (max 10MB).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The uploaded photo details.</returns>
    /// <response code="200">Returns the uploaded photo.</response>
    /// <response code="400">Invalid file or no file uploaded.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost("add-photo")]
    [RequestSizeLimit(10_000_000)]
    [ProducesResponseType(typeof(PhotoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddPhoto(IFormFile file, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var member = await _memberService.GetByUserIdAsync(userId, cancellationToken);
        if (member is null) return NotFound("Member profile not found");

        if (file is null || file.Length <= 0) return BadRequest("No file uploaded");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension)) return BadRequest("Invalid file type. Allowed: jpg, jpeg, png, gif, webp");

        using var stream = file.OpenReadStream();

        var photo = await _memberService.AddPhotoAsync(
            member.Id,
            stream,
            file.FileName,
            false, // isMain
            cancellationToken);

        return Ok(photo.ToPhotoResponse());
    }

    /// <summary>
    /// Sets a photo as the main/profile photo for the authenticated member.
    /// </summary>
    /// <param name="photoId">The photo's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Photo set as main successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Photo not found.</response>
    [HttpPut("set-main-photo/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetMainPhoto([FromRoute] string photoId, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var member = await _memberService.GetByUserIdAsync(userId, cancellationToken);
        if (member is null) return NotFound("Member profile not found");

        var result = await _memberService.SetMainPhotoAsync(member.Id, photoId, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a photo for the authenticated member.
    /// </summary>
    /// <param name="photoId">The photo's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Photo deleted successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Photo not found.</response>
    [HttpDelete("delete-photo/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePhoto([FromRoute] string photoId, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var member = await _memberService.GetByUserIdAsync(userId, cancellationToken);
        if (member is null) return NotFound("Member profile not found");

        var result = await _memberService.RemovePhotoAsync(member.Id, photoId, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
