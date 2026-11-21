using Lunex.Application.Member.Commands;
using Lunex.Domain.Member.Repositories;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class SetMainPhotoCommandHandler(
    IMemberRepository memberRepository) : IRequestHandler<SetMainPhotoCommand, bool>
{
    public async Task<bool> Handle(SetMainPhotoCommand command, CancellationToken cancellationToken)
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

        member.SetMainPhoto(command.PhotoId);

        await memberRepository.UpdateAsync(member, cancellationToken);

        return true;
    }
}