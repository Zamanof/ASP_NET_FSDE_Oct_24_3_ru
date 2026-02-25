using ASP_NET_21._TaskFlow.API.Authorization;
using ASP_NET_21._TaskFlow.API.Storage;
using ASP_NET_21._TaskFlow.API.Validators;
using ASP_NET_21._TaskFlow.BLL.Config;
using ASP_NET_21._TaskFlow.BLL.Mapping;
using ASP_NET_21._TaskFlow.BLL.Services;
using ASP_NET_21._TaskFlow.BLL.Storage;
using ASP_NET_21._TaskFlow.DAL;
using ASP_NET_21._TaskFlow.Data;
using ASP_NET_21._TaskFlow.Models;
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

namespace ASP_NET_21._TaskFlow.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddSwaggerGen(
                 options =>
                 {
                     options.SwaggerDoc("v1",
                     new OpenApiInfo
                     {
                         Version = "v1",
                         Title = "TaskFlow API",
                         Description = "This API includes full CRUD operations for the TaskFlow project.",
                         Contact = new OpenApiContact
                         {
                             Name = "TaskFlow Team",
                             Email = "support@taskflow.com"
                         },
                         License = new OpenApiLicense
                         {
                             Name = "MIT License",
                             Url = new Uri("https://opensource.org/license/mit")
                         }
                     });

                     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                     if (File.Exists(xmlPath))
                     {
                         options.IncludeXmlComments(xmlPath);
                     }

                     // JWT options for Swaggeer
                     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                     {
                         Description = """
                JWT Suthorization header using the Bearer scheme. 
                Example: Authorization: Bearer {token}
                """,
                         Name = "Authorization",
                         In = ParameterLocation.Header,
                         Type = SecuritySchemeType.ApiKey,
                         Scheme = "Bearer"
                     });

                     options.AddSecurityRequirement(
                         new OpenApiSecurityRequirement
                         {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                         });
                 });

        return services;
    }

    public static IServiceCollection AddTaskflowDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskFlowDBConnetionString");
        services.AddDbContext<TaskFlowDbContext>(
            options => options.UseSqlServer(connectionString)
            );
        return services;
    }

    public static IServiceCollection AddIdentityAndDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));
        services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    }

)
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddJwtAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);


        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
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
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey!)),
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        services.AddAuthorization(
            options =>
            {
                options
                .AddPolicy(
                    "AdminOnly",
                    policy =>
                        policy.RequireRole("Admin"));
                options
                .AddPolicy(
                    "ManagerOrAdmin",
                    policy =>
                        policy.RequireRole("Admin", "Manager"));
                options
                .AddPolicy(
                    "UserOrAbove",
                    policy =>
                        policy.RequireRole("Admin", "Manager", "User"));
                options
                 .AddPolicy(
                    "ProjectOwnerOrAdmin",
                    policy =>
                        policy.Requirements.Add(new ProjectOwnerOrAdminRequirement()));
                options
                 .AddPolicy(
                    "ProjectMemberOrHigher",
                    policy =>
                        policy.Requirements.Add(new ProjectMemberOrHigherRequirment()));
                options
                 .AddPolicy(
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
        this IServiceCollection services)
    {
        services.AddCors(
    options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.WithOrigins(
                    "http://localhost:3000",
                    "http://127.0.0.1:3000"
                    )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            }
            );
    }
    );
        return services;
    }

    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services)
    {

        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    public static IServiceCollection AddAutoMapperAndOtherDI(
       this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        // Services
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileStorage, LocalDiskStorage>();
        services.AddScoped<ITaskAttachmentService, TaskAttachmentService>();
        // DAL repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        services.AddScoped<ITaskAttachmentRepository, TaskAttachmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
