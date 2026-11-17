namespace Lunex.Application.Common.Pagination;

/// <summary>
/// Base pagination parameters for API requests.
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Default is 10, maximum is 50.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
    }
}

/// <summary>
/// Member-specific pagination and filter parameters.
/// </summary>
public sealed class MemberParams : PaginationParams
{
    /// <summary>
    /// Filter members by gender (e.g., "Male", "Female").
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Exclude a specific member from results by ID (e.g., current user).
    /// </summary>
    public string? CurrentMemberId { get; set; }
}
