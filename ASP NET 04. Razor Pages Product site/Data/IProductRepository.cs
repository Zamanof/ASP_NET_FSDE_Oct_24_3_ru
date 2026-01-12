using ASP_NET_04._Razor_Pages_Product_site.Models;

namespace ASP_NET_04._Razor_Pages_Product_site.Data;

public interface IProductRepository
{
    public Task<Product> AddProductAsync(Product product);
    public Task<Product> GetProductByIdAsync(int id);
    public Task<IEnumerable<Product>> GetProductsAsync();
}
