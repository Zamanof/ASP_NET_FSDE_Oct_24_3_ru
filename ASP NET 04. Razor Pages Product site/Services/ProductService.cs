using ASP_NET_04._Razor_Pages_Product_site.Data;
using ASP_NET_04._Razor_Pages_Product_site.Models;
using Bogus;

namespace ASP_NET_04._Razor_Pages_Product_site.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetProductsAsync()
        => _repository.GetProductsAsync();

    public Task<Product> GetProductByIdAsync(int id)
        => _repository.GetProductByIdAsync(id);

    public Product AddProduct(Product product)
    {
        var faker = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.Random.Int(1));

        product.Id = faker.Generate().Id;

        if (product.Count > 0) product.IsAvailable = true;

        _repository.AddProductAsync(product);

        return product;
    }
}
