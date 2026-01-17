using ASP_NET_05._ASP_Filters.Filters;
using ASP_NET_05._ASP_Filters.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_NET_05._ASP_Filters.Controllers;

//[TypeFilter(typeof(MyAuthorizationFilter))]
//[TypeFilter(typeof(ApiKeyQueryFilter))]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        //int number = 0;
        //var result = 198 / number;
        return View();
    }

    [TypeFilter(typeof(MyAuthorizationFilter))]
    public IActionResult Welcome()
    {
        return View();
    }

    [LastEnterDate]
    public IActionResult Privacy()
    {
        throw new KeyNotFoundException();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
