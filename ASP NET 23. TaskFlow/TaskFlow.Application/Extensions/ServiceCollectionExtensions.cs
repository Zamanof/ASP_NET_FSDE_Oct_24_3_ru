using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Mapping;
using TaskFlow.Application.Services;
using TaskFlow.Application.Validators;

namespace TaskFlow.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        return services;
    }
}