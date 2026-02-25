using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskFlow.Api.Authorization;
using TaskFlow.Application.Config;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TaskFlow API",
                Description = "This API includes full CRUD operations for the TaskFlow project.",
                Contact = new OpenApiContact { Name = "TaskFlow Team", Email = "support@taskflow.com" },
                License = new OpenApiLicense { Name = "MIT License", Url = new Uri("https://opensource.org/license/mit") }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: Authorization: Bearer {token}",
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
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddJwtAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("AdminOrManager", policy => policy.RequireRole("Admin", "Manager"));
            options.AddPolicy("UserOrAbove", policy => policy.RequireRole("Admin", "Manager", "User"));
            options.AddPolicy("ProjectOwnerOrAdmin", policy => policy.Requirements.Add(new ProjectOwnerOrAdminRequirement()));
            options.AddPolicy("ProjectMemberOrHigher", policy => policy.Requirements.Add(new ProjectMemberOrHigherRequirement()));
            options.AddPolicy("TaskStatusChange", policy => policy.Requirements.Add(new TaskStatusChangeRequirement()));
        });

        services.AddScoped<IAuthorizationHandler, ProjectOwnerOrAdminHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMemberOrHigherHandler>();
        services.AddScoped<IAuthorizationHandler, TaskStatusChangeHandler>();

        return services;
    }

    public static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<TaskFlowDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        return services;
    }
}
