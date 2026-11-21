using Lunex.Domain.Common;
using Lunex.Domain.Identity.ValueObjects;

namespace Lunex.Domain.Identity.Events;

public sealed record UserRegistered : DomainEventBase
{
    public string UserId { get; init; } = string.Empty;
    public Email Email { get; init; }
}