using ASP_NET_03._Dependency_Injection.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// IoC, Ninject, ...
builder.Services.AddSingleton<IProductRepository, InMemoryRepository>();
//builder.Services.AddSingleton<IProductRepository>(new InMemoryRepository());
builder.Services.Add(
    new ServiceDescriptor(
        typeof(IProductRepository),
        typeof(InMemoryRepository),
        ServiceLifetime.Singleton));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
