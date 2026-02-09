namespace Lunex.Application.Common.Services.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
