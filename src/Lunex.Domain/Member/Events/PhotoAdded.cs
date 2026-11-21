using Lunex.Domain.Common;

namespace Lunex.Domain.Member.Events;

public sealed record PhotoAdded : DomainEventBase
{
    public string MemberId { get; init; } = string.Empty;
    public string PhotoId { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public bool IsMain { get; init; }
}