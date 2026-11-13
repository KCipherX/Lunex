using Lunex.Contracts.Members.Responses;

namespace Lunex.Application.Common.Services.Abstractions;

public interface IPhotoService
{
    Task<PhotoUploadResult> UploadPhotoAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken);
    Task<bool> DeletePhotoAsync(string publicId, CancellationToken cancellationToken);
}