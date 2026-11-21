using Lunex.Domain.Common;

namespace Lunex.Domain.Member.Events;

public sealed record PhotoRemoved : DomainEventBase
{
    public string MemberId { get; init; } = string.Empty;
    public string PhotoId { get; init; } = string.Empty;
}