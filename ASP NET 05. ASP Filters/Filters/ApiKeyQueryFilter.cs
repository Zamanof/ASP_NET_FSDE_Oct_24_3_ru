using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_NET_05._ASP_Filters.Filters;

public class ApiKeyQueryFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAutorized =
            context
            .HttpContext
            .Request
            .Query
            .Any(q => q.Key == "apiKey" && q.Value == "qwerty123456");

        if (!isAutorized)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
