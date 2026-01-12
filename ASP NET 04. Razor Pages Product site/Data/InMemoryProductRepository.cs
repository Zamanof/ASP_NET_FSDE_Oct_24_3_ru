using ASP_NET_04._Razor_Pages_Product_site.Models;
using Bogus;

namespace ASP_NET_04._Razor_Pages_Product_site.Data;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    // Bogus

    public InMemoryProductRepository()
    {
        var faker = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.Random.Int(1))
            .RuleFor(p => p.Name, f => f.Commerce.Product())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Count, f => f.Random.UInt(1))
            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 30))
            .RuleFor(p => p.IsAvailable, true);
        _products.AddRange(faker.GenerateBetween(30, 30));
    }
    public Task<Product> AddProductAsync(Product product)
    {
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product> GetProductByIdAsync(int id)
        => Task.FromResult(_products.FirstOrDefault(p => p.Id == id))!;

    public Task<IEnumerable<Product>> GetProductsAsync()
        => Task.FromResult(_products.AsEnumerable());
}
