using Lunex.Domain.Common;

namespace Lunex.Domain.Member.Events;

public sealed record MemberActivityUpdated : DomainEventBase
{
    public string MemberId { get; init; } = string.Empty;
    public DateTime LastActiveAt { get; init; }
}