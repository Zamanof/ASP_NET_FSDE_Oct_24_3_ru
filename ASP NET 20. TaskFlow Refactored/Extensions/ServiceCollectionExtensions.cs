using ASP_NET_20._TaskFlow_Refactored.Authorization;
using ASP_NET_20._TaskFlow_Refactored.Config;
using ASP_NET_20._TaskFlow_Refactored.Data;
using ASP_NET_20._TaskFlow_Refactored.Mappings;
using ASP_NET_20._TaskFlow_Refactored.Models;
using ASP_NET_20._TaskFlow_Refactored.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace ASP_NET_20._TaskFlow_Refactored.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSwagger(
        this IServiceCollection services
        )
    {
        services.AddControllers();
        services.AddOpenApi();

        services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TaskFlow API",
                    Description = "API for project and task management. This API provides a full set of operations for working with projects and tasks.",
                    Contact = new OpenApiContact
                    {
                        Name = "TaskFlow Team",
                        Email = "support@taskflow.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT Licence",
                        Url = new Uri("https://opensource.org/license/mit")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // JWT configuration for swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = """
            JWT Authorization header 
            using Bearer scheme.
            Example: Bearer {token}
            """,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }, Array.Empty<string>()
            }
                });
            }
            );

        return services;
    }

    public static IServiceCollection AddTaskFlowDbContext(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        var connectionString = configuration
                        .GetConnectionString("DefaultConnectionString");


        services.AddDbContext<TaskFlowDbContext>(
            options => options.UseSqlServer(connectionString)
            );
        return services;
    }

    public static IServiceCollection AddIdentityAndDb(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services
                .Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        services.AddIdentity<ApplicationUser, IdentityRole>(
            options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<TaskFlowDbContext>()
                .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        // JWT Authentication
        var jwtConfig = new JwtConfig();
        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);

        services
            .AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            }
            );

        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    "AdminOnly",
                    policy =>
                        policy.RequireRole("Admin"));

                options.AddPolicy(
                    "AdminOrManager",
                    policy =>
                        policy.RequireRole("Admin", "Manager"));

                options.AddPolicy(
                    "UserOrAbove",
                    policy =>
                        policy.RequireRole("Admin", "Manager", "User"));

                options.AddPolicy(
                    "ProjectOwnerOrAdmin",
                    policy =>
                        policy.Requirements.Add(new ProjectOwnerOrAdminRequirement()));

                options.AddPolicy(
                    "ProjectMemberOrHigher",
                    policy =>
                        policy.Requirements.Add(new ProjectMemberOrHigherRequirment()));

                options.AddPolicy(
                    "TaskStatusChange",
                    policy =>
                        policy.Requirements.Add(new TaskStatusChangeRequirement()));
            }
            );
        services.AddScoped<IAuthorizationHandler, ProjectOwnerOrAdminHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberOrHigherHandler>();
        services.AddScoped<IAuthorizationHandler, TaskStatusChangeHandler>();
        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services
        )
    {
        services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://localhost:3000",
                            "http://127.0.0.1:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    }
                    );
            }
    );

        return services;
    }


    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services
        )
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    public static IServiceCollection AddAutoMapperAndOtherServices(
       this IServiceCollection services
       )
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
