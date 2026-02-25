using ASP_NET_21._TaskFlow.API.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger()
                .AddTaskflowDbContext(builder.Configuration)
                .AddIdentityAndDb(builder.Configuration)
                .AddJwtAuthenticationAndAuthorization(builder.Configuration)
                .AddCorsPolicy()
                .AddFluentValidation()
                .AddAutoMapperAndOtherDI();

var app = builder.Build();

app.UseTaskFlowPipeLine();

await app.EnsureRolesSeededAsync();

app.Run();