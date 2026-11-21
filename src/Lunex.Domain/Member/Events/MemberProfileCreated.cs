using Lunex.Domain.Common;
using Lunex.Domain.Member.ValueObjects;

namespace Lunex.Domain.Member.Events;

public sealed record MemberProfileCreated : DomainEventBase
{
    public string MemberId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Gender Gender { get; init; }
    public DateOfBirth DateOfBirth { get; init; }
    public Location Location { get; init; }
}