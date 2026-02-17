namespace Discounts.Application.Models;

public sealed class PagedResult<T>
{
    public int TotalCount { get; }
    public IReadOnlyList<T> Items { get; }

    public int PageSize { get; }
    public int PageNumber { get; }

    public bool HasNext => PageNumber * PageSize < TotalCount;
    public bool HasPrevious => PageNumber > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}