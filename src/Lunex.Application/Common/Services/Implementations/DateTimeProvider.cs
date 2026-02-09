using Lunex.Application.Common.Services.Abstractions;

namespace Lunex.Application.Common.Services.Implementations;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
