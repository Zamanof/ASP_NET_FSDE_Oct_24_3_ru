using ASP_NET_04._Razor_Pages_Product_site.Models;
using ASP_NET_04._Razor_Pages_Product_site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_04._Razor_Pages_Product_site.Pages;

public class IndexModel : PageModel
{
    private readonly ProductService _service;

    public IndexModel(ProductService service)
    {
        _service = service;
    }

    public void OnPost(Product product)
    {
        _service.AddProduct(product);
    }
}
