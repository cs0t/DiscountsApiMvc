namespace Discounts.Application.Queries;

public interface IPagedQuery
{
    int  PageNumber { get; init; }
    int PageSize { get; init; }
}