using ASP_NET_03._Dependency_Injection.Models;

namespace ASP_NET_03._Dependency_Injection.Data;

public class InMemoryRepository: IProductRepository
{
    private readonly IDictionary<Guid, Product> _products
        = new Dictionary<Guid, Product>();
    public InMemoryRepository()
    {
        AddProduct(new Product { Name = "Oil", Description="From Venesuella without Madura" });
        AddProduct(new Product { Name = "Vegetable Oil", Description="Final Qarqidali" });
        AddProduct(new Product { Name = "Baby Oil", Description="Jonson & Jonson" });
        AddProduct(new Product { Name = "Nehre Oil", Description="Kendden Metbexe" });
        AddProduct(new Product { Name = "Nobel Peace", Description="Kak tebe (Ilon Musk) Trump " });
    }
    public Product AddProduct(Product product)
    {
        product.Id = Guid.NewGuid();
        _products.Add(product.Id, product);
        return product;
    }

    public IEnumerable<Product> GetProducts() => _products.Values;
}
