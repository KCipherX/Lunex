namespace Lunex.Application.Common.Pagination;

public sealed class PaginatedResult<T>
{
    public PaginationMetaData Metadata { get; set; } = default!;
    public List<T> Items { get; set; } = [];
}

public sealed class PaginationMetaData
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}       