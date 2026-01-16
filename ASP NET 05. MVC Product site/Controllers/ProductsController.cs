using ASP_NET_05._MVC_Product_site.Data;
using ASP_NET_05._MVC_Product_site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASP_NET_05._MVC_Product_site.Controllers;

public class ProductsController : Controller
{
    private readonly ProductContext _context;

    public ProductsController(ProductContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> IndexAsync()
    {
        return View(await _context.Products.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateAsync(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View();
    }

    public async Task<IActionResult> Details(int? id){
        if (id is null) return NotFound();
        var product 
            = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();
        return View(product);
    }
}
/*
 Razor pages:
    Get all Products (GET) -> html
    Create Product (POST) -> html

MVC:
    Get all Products:
        (GET) -> html
    Create Product:
        (GET) -> html
        (POST) -> html
 
 */