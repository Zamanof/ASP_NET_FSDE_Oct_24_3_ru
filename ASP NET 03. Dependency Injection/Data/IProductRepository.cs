using ASP_NET_03._Dependency_Injection.Models;

namespace ASP_NET_03._Dependency_Injection.Data;

public interface IProductRepository
{
    public Product AddProduct(Product product);
    public IEnumerable<Product> GetProducts();
}
