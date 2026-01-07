using ASP_NET_02._Mini_ASP.Interfaces;
using ASP_NET_02._Mini_ASP.Middlewares;

internal class Startup : IStartup
{
    public void Configure(MiddlewareBuilder builder)
    {
        builder.Use<LoggerMiddlware>();
        builder.Use<StaticFilesMiddlware>();
    }
}