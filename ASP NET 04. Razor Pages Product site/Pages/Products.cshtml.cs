using ASP_NET_04._Razor_Pages_Product_site.Models;
using ASP_NET_04._Razor_Pages_Product_site.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_04._Razor_Pages_Product_site.Pages;

public class ProductsModel : PageModel
{
    private readonly ProductService _service;
    public IEnumerable<Product> Products { get; set; }
        = Enumerable.Empty<Product>(); 

    public ProductsModel(ProductService service)
    {
        _service = service;
    }

    public async Task OnGet()
    {
        Products = await _service.GetProductsAsync();
    }
}
