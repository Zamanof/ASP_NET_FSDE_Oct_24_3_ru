namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages
        => Convert.ToInt32(Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious 
        => Page > 1;
    public bool HasNext
        => Page < TotalPages;

    public static PagedResult<T> Create(
                                        IEnumerable<T> items,
                                        int page,
                                        int pageSize,
                                        int totalCount)
    {
        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
