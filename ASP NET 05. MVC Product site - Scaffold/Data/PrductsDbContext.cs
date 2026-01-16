using ASP_NET_05._MVC_Product_site___Scaffold.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_05._MVC_Product_site___Scaffold.Data;

public class PrductsDbContext:DbContext
{
    public PrductsDbContext(DbContextOptions options)
    : base(options)
    { }

    public DbSet<Product> Products => Set<Product>();
}
