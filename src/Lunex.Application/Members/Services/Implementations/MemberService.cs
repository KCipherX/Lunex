using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Members.Mappers;
using Lunex.Application.Members.Persistence.Abstractions;
using Lunex.Application.Members.Services.Abstractions;
using Lunex.Contracts.Members.Requests;
using Lunex.Contracts.Members.Responses;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Application.Members.Services.Implementations;

public sealed class MemberService(IMemberRepository memberRepository, IPhotoService photoService) : IMemberService
{
    public async Task<IReadOnlyList<MemberResponse>> GetAsync(CancellationToken cancellationToken)
    {
        var members = await memberRepository.GetAsync(cancellationToken);
        return members.ToResponse();
    }

    public async Task<MemberResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(id, cancellationToken);
        return member?.ToResponse();
    }

    public async Task<IReadOnlyList<PhotoResponse>> GetPhotosAsync(string id, CancellationToken cancellationToken)
    {
        var memberPhotos = await memberRepository.GetPhotosAsync(id, cancellationToken);
        return memberPhotos.ToResponse();
    }

    public async Task<MemberResponse?> UpdateAsync(string memberId, MemberUpdateRequest request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(memberId, cancellationToken);
        if (member is null)
        {
            return null;
        }

        member.Name = request.DisplayName;
        member.Description = request.Description;
        member.City = request.City;
        member.Country = request.Country;

        await memberRepository.UpdateAsync(member, cancellationToken);

        return member.ToResponse();
    }

    public async Task<PhotoResponse> UploadPhotoAsync(string memberId, Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        var uploadResult = await photoService.UploadPhotoAsync(
            fileStream,
            fileName,
            $"members/{memberId}",
            cancellationToken);

        var photo = new Photo
        {
            Url = uploadResult.Url,
            PublicId = uploadResult.PublicId
        };

        await memberRepository.AddPhotoAsync(
            memberId,
            photo,
            cancellationToken);

        return photo.ToResponse();
    }

    public async Task<bool> DeletePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken)
    {
        var photos = await memberRepository.GetPhotosAsync(
            memberId,
            cancellationToken);

        var photo = photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null)
            return false;

        if (!string.IsNullOrEmpty(photo.PublicId))
            await photoService.DeletePhotoAsync(photo.PublicId, cancellationToken);

        return await memberRepository.RemovePhotoAsync(
            memberId,
            photoId,
            cancellationToken);
    }
}
