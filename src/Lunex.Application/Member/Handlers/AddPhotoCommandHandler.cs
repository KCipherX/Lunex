using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Member.Commands;
using Lunex.Application.Member.Results;
using Lunex.Domain.Member.Repositories;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class AddPhotoCommandHandler(
    IMemberRepository memberRepository,
    IPhotoService photoService) : IRequestHandler<AddPhotoCommand, PhotoResult>
{
    public async Task<PhotoResult> Handle(AddPhotoCommand command, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(command.MemberId, cancellationToken);
        if (member is null)
        {
            throw new InvalidOperationException("Member not found");
        }

        var uploadResult = await photoService.UploadPhotoAsync(
            command.FileStream,
            command.FileName,
            $"members/{command.MemberId}",
            cancellationToken);

        var photo = member.AddPhoto(uploadResult.Url, uploadResult.PublicId, command.IsMain);

        await memberRepository.UpdateAsync(member, cancellationToken);

        return PhotoResult.FromDomain(photo);
    }
}