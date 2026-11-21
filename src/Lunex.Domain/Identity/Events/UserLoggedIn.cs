using Lunex.Domain.Common;

namespace Lunex.Domain.Identity.Events;

public sealed record UserLoggedIn : DomainEventBase
{
    public string UserId { get; init; } = string.Empty;
    public DateTime LoginTime { get; init; } = DateTime.UtcNow;
}