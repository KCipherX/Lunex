namespace Lunex.Domain.Common.Pagination;

/// <summary>
/// Represents a paginated result set with metadata.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PaginatedResult<T>
{
    /// <summary>
    /// Pagination metadata.
    /// </summary>
    public PaginationMetaData Metadata { get; set; } = default!;

    /// <summary>
    /// The items for the current page.
    /// </summary>
    public List<T> Items { get; set; } = [];
}

/// <summary>
/// Metadata about the pagination state.
/// </summary>
public sealed class PaginationMetaData
{
    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}