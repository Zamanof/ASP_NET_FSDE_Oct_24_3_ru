
using ASP_NET_05._MVC_Product_site.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_05._MVC_Product_site.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions options) 
        : base(options)
    {}

    public DbSet<Product> Products => Set<Product>();
}
