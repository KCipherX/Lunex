using Lunex.Domain.Common;
using Lunex.Domain.Member.ValueObjects;

namespace Lunex.Domain.Member.Events;

public sealed record MemberProfileUpdated : DomainEventBase
{
    public string MemberId { get; init; } = string.Empty;
    public string? Name { get; init; }
    public Gender? Gender { get; init; }
    public DateOfBirth? DateOfBirth { get; init; }
    public Location? Location { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
}