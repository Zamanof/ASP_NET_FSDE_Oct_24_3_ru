using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_NET_05._ASP_Filters.Filters;

public class LastEnterDateAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuted(ResourceExecutedContext context)
    {}

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context
            .HttpContext
            .Response
            .Cookies
            .Append("MyLastVisit", DateTime.Now.ToString("dd-MM-yyyy-HH:mm:ss"));
    }
}
