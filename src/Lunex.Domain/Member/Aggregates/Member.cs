using Lunex.Domain.Common;
using Lunex.Domain.Member.Events;
using Lunex.Domain.Member.ValueObjects;

namespace Lunex.Domain.Member.Aggregates;

public sealed class Member : IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly List<Photo> _photos = [];

    public string Id { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Gender Gender { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public Location Location { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActiveAt { get; private set; }

    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Member() { }

    public static Member CreateProfile(
        string userId,
        string name,
        Gender gender,
        DateOfBirth dateOfBirth,
        Location location,
        string? description = null,
        string? imageUrl = null)
    {
        var member = new Member
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Name = name,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            Location = location,
            Description = description ?? string.Empty,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow
        };

        member._domainEvents.Add(new MemberProfileCreated
        {
            MemberId = member.Id,
            UserId = userId,
            Name = name,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            Location = location
        });

        return member;
    }

    public void UpdateProfile(
        string? name = null,
        Gender? gender = null,
        DateOfBirth? dateOfBirth = null,
        Location? location = null,
        string? description = null,
        string? imageUrl = null)
    {
        var hasChanges = false;

        if (name != null && name != Name)
        {
            Name = name;
            hasChanges = true;
        }

        if (gender.HasValue && gender.Value != Gender)
        {
            Gender = gender.Value;
            hasChanges = true;
        }

        if (dateOfBirth.HasValue && dateOfBirth.Value != DateOfBirth)
        {
            DateOfBirth = dateOfBirth.Value;
            hasChanges = true;
        }

        if (location.HasValue && location.Value != Location)
        {
            Location = location.Value;
            hasChanges = true;
        }

        if (description != null && description != Description)
        {
            Description = description;
            hasChanges = true;
        }

        if (imageUrl != null && imageUrl != ImageUrl)
        {
            ImageUrl = imageUrl;
            hasChanges = true;
        }

        if (hasChanges)
        {
            _domainEvents.Add(new MemberProfileUpdated
            {
                MemberId = Id,
                Name = name,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                Location = location,
                Description = description,
                ImageUrl = imageUrl
            });
        }
    }

    public Photo AddPhoto(string url, string? publicId = null, bool isMain = false)
    {
        var photo = Photo.Create(url, publicId, isMain);

        if (isMain)
        {
            foreach (var existingPhoto in _photos)
            {
                existingPhoto.UnsetAsMain();
            }
        }
        else if (_photos.Count == 0)
        {
            photo.SetAsMain();
        }

        _photos.Add(photo);

        _domainEvents.Add(new PhotoAdded
        {
            MemberId = Id,
            PhotoId = photo.Id,
            Url = photo.Url,
            IsMain = photo.IsMain
        });

        return photo;
    }

    public void RemovePhoto(string photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null)
            throw new InvalidOperationException($"Photo with ID {photoId} not found");

        var wasMain = photo.IsMain;
        _photos.Remove(photo);

        if (wasMain && _photos.Count > 0)
        {
            _photos[0].SetAsMain();
        }

        _domainEvents.Add(new PhotoRemoved
        {
            MemberId = Id,
            PhotoId = photoId
        });
    }

    public void SetMainPhoto(string photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null)
            throw new InvalidOperationException($"Photo with ID {photoId} not found");

        foreach (var p in _photos)
        {
            p.UnsetAsMain();
        }

        photo.SetAsMain();
    }

    public void UpdateActivity()
    {
        LastActiveAt = DateTime.UtcNow;
        _domainEvents.Add(new MemberActivityUpdated
        {
            MemberId = Id,
            LastActiveAt = LastActiveAt
        });
    }

    public int GetAge() => DateOfBirth.CalculateAge();

    public void ClearDomainEvents() => _domainEvents.Clear();
}