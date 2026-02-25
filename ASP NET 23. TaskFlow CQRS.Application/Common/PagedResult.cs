namespace ASP_NET_23._TaskFlow_CQRS.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => Convert.ToInt32(Math.Ceiling(TotalCount / (double)PageSize));
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public static PagedResult<T> Create(IEnumerable<T> items, int page, int pageSize, int totalCount) =>
        new()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
