using FluentValidation.AspNetCore;
using TaskFlow.Api.Extensions;
using TaskFlow.Application.Extensions;
using TaskFlow.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddIdentityAndJwt(builder.Configuration);

builder.Services.AddSwagger();
builder.Services.AddJwtAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

app.UseTaskFlowPipeLine();
await app.EnsureRolesSeededAsync();

app.Run();
