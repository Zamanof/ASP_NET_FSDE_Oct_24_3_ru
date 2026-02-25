using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Config;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Storage;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Infrastructure.Jwt;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Storage;

namespace TaskFlow.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        var connectionString = configuration.GetConnectionString("TaskFlowDBConnetionString");
        services.AddDbContext<TaskFlowDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITaskAttachmentRepository, TaskAttachmentRepository>();

        services.AddScoped<IFileStorage, LocalDiskStorage>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthUserStore, AuthUserStore>();

        return services;
    }
}
