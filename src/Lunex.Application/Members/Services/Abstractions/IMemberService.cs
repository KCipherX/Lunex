using Lunex.Contracts.Members.Requests;
using Lunex.Contracts.Members.Responses;
using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Application.Members.Services.Abstractions;

public interface IMemberService
{
    Task<IReadOnlyList<MemberResponse>> GetAsync(CancellationToken cancellationToken);
    Task<MemberResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PhotoResponse>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task<MemberResponse?> UpdateAsync(string memberId, MemberUpdateRequest request, CancellationToken cancellationToken);
    Task<PhotoResponse> UploadPhotoAsync(string memberId, Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<bool> DeletePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
}
