using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Lunex.Application.Common.Configurations;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Contracts.Members.Responses;

using Microsoft.Extensions.Options;

namespace Lunex.Infrastructure.Services;

public sealed class PhotoService(IOptions<CloudinarySettings> options) : IPhotoService
{
    private readonly Cloudinary _cloudinary = new(new Account(
        options.Value.CloudName,
        options.Value.ApiKey,
        options.Value.ApiSecret))
    {
        Api = { Secure = true }
    };

    public async Task<PhotoUploadResult> UploadPhotoAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        var uploadParams = new ImageUploadParams
        {
            File = new(fileName, fileStream),
            Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
            Folder = folder
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return new PhotoUploadResult(
            uploadResult.Url.ToString(),
            uploadResult.PublicId,
            uploadResult.Width,
            uploadResult.Height,
            uploadResult.Format);
    }

    public async Task<bool> DeletePhotoAsync(string publicId, CancellationToken cancellationToken)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        return result.Result.Equals("ok");
    }
}