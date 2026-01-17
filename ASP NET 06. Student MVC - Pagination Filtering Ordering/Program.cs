using ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StudentsDbContext>(
    option=> 
    option
    .UseSqlServer(builder.Configuration.GetConnectionString("StudentsSiteDbCS"),
    setting=>
    {
        setting.CommandTimeout(30);
        setting.MigrationsHistoryTable("EF_TABLE_MIGRATIONS");
    }
    )
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Students}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
