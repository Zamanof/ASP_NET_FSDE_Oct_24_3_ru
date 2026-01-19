namespace ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Models;

public class PginationViewModel<TModel>
{
    public IEnumerable<TModel> Items  { get;}
    public int Page { get;}
    public int PageSize { get;}
    public int TotalCount { get;}

    public PginationViewModel(
        IEnumerable<TModel> items, 
        int page, 
        int pageSize, 
        int count)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = Convert.ToInt32(Math.Ceiling((float)count / pageSize));
        // 12/5 = 2.4 => 3
    }
}
