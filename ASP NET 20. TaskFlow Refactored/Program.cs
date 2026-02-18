using ASP_NET_20._TaskFlow_Refactored.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger()
                .AddTaskFlowDbContext(builder.Configuration)
                .AddIdentityAndDb(builder.Configuration)
                .AddAuthenticationAndAuthorization(builder.Configuration)
                .AddCorsPolicy()
                .AddFluentValidation()
                .AddAutoMapperAndOtherServices();
                
var app = builder.Build();

app.UseTaskFlowPipeLine();

await app.EnsureRolesSeededAsync();

app.Run();