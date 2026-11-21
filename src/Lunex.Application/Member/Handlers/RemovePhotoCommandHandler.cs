using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Member.Commands;
using Lunex.Domain.Member.Repositories;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class RemovePhotoCommandHandler(
    IMemberRepository memberRepository,
    IPhotoService photoService) : IRequestHandler<RemovePhotoCommand, bool>
{
    public async Task<bool> Handle(RemovePhotoCommand command, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(command.MemberId, cancellationToken);
        if (member is null)
        {
            return false;
        }

        var photo = member.Photos.FirstOrDefault(p => p.Id == command.PhotoId);
        if (photo is null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(photo.PublicId))
        {
            await photoService.DeletePhotoAsync(photo.PublicId, cancellationToken);
        }

        member.RemovePhoto(command.PhotoId);

        await memberRepository.UpdateAsync(member, cancellationToken);

        return true;
    }
}