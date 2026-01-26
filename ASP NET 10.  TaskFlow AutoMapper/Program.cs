using ASP_NET_10.__TaskFlow_AutoMapper.Data;
using ASP_NET_10.__TaskFlow_AutoMapper.Mappings;
using ASP_NET_10.__TaskFlow_AutoMapper.Services;
using ASP_NET_10.__TaskFlow_AutoMapper.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var connectionString = builder
                        .Configuration
                        .GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<TaskFlowDbContext>(
    options => options.UseSqlServer(connectionString)
    );

builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
