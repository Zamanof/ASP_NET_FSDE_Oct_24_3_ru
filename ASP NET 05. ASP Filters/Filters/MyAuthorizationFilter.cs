using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_NET_05._ASP_Filters.Filters;

public class MyAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isLogin = context
            .HttpContext
            .Request
            .Query
            .Any(q => q.Key == "Name" && q.Value == "Admin");

        var isPassword = context
            .HttpContext
            .Request
            .Query
            .Any(q => q.Key == "Password" && q.Value == "P@ss12345");

        if(!isLogin || !isPassword)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
