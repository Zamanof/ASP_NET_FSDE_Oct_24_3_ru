using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ASP_NET_23._TaskFlow_CQRS.Application.Mapping;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using ASP_NET_23._TaskFlow_CQRS.Application.Validators;
using MediatR;
using ASP_NET_23._TaskFlow_CQRS.Application.Common.Behaivors;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Extensions;

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

        services.AddMediatR(
            config=> 
            config.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaivor<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaivor<,>));


        return services;
    }
}