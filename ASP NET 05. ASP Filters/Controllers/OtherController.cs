using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_05._ASP_Filters.Controllers;

public class OtherController : Controller
{
    public IActionResult Index()
    {
        throw new NullReferenceException();
        return View();
    }
}
