using ASP_NET_03._Dependency_Injection___service;
using ASP_NET_03._Dependency_Injection___service.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_03._Dependency_Injection.Pages;

public class IndexModel : PageModel
{

    private readonly ProductService _service;

    public IndexModel(ProductService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        var products = _service.GetProducts();
        ViewData["Products"] = products;
    }
}
