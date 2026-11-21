using Lunex.Application.Member.Commands;
using Lunex.Application.Member.Queries;
using Lunex.Application.Member.Results;
using Lunex.Domain.Common.Pagination;

using MediatR;

namespace Lunex.Application.Member.Services;

public sealed class MemberService(IMediator mediator) : IMemberService
{
    public async Task<PaginatedResult<MemberResult>> GetAsync(MemberParams memberParams, CancellationToken cancellationToken)
    {
        var query = new GetMembersQuery(memberParams);
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<MemberResult?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var query = new GetMemberByIdQuery(id);
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<MemberResult?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        var query = new GetMemberByUserIdQuery(userId);
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<MemberResult> CreateProfileAsync(string userId, string name, string gender, DateOnly dateOfBirth, string city, string country, string? description, CancellationToken cancellationToken)
    {
        var command = new CreateMemberProfileCommand(userId, name, gender, dateOfBirth, city, country, description);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<MemberResult?> UpdateProfileAsync(string memberId, string? name, string? gender, DateOnly? dateOfBirth, string? city, string? country, string? description, string? imageUrl, CancellationToken cancellationToken)
    {
        var command = new UpdateMemberProfileCommand(memberId, name, gender, dateOfBirth, city, country, description, imageUrl);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<IReadOnlyList<PhotoResult>> GetPhotosAsync(string id, CancellationToken cancellationToken)
    {
        var member = await GetByIdAsync(id, cancellationToken);
        return member?.Photos ?? [];
    }

    public async Task<PhotoResult> AddPhotoAsync(string memberId, Stream fileStream, string fileName, bool isMain, CancellationToken cancellationToken)
    {
        var command = new AddPhotoCommand(memberId, fileStream, fileName, isMain);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<bool> RemovePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken)
    {
        var command = new RemovePhotoCommand(memberId, photoId);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<bool> SetMainPhotoAsync(string memberId, string photoId, CancellationToken cancellationToken)
    {
        var command = new SetMainPhotoCommand(memberId, photoId);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task UpdateLastActiveAsync(string memberId, CancellationToken cancellationToken)
    {
        var member = await GetByIdAsync(memberId, cancellationToken);
        if (member is not null)
        {
            var command = new UpdateMemberProfileCommand(memberId, ImageUrl: member.ImageUrl);
            await mediator.Send(command, cancellationToken);
        }
    }
}