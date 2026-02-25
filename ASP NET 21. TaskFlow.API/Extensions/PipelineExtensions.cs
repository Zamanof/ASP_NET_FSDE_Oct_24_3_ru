using ASP_NET_21._TaskFlow.API.Middlewares;
using ASP_NET_21._TaskFlow.Data;

namespace ASP_NET_21._TaskFlow.API.Extensions;

public static class PipelineExtensions
{
    public static WebApplication UseTaskFlowPipeLine(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskFlow API v1");
                    options.RoutePrefix = string.Empty;
                    options.DisplayRequestDuration();
                    options.EnableFilter();
                    options.EnableTryItOutByDefault();
                    options.EnablePersistAuthorization();
                }
                );
            app.MapOpenApi();

        }
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseCors();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static async Task EnsureRolesSeededAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
    }

}
