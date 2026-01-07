using ASP_NET_02._Mini_ASP.Interfaces;
using System.Net;

namespace ASP_NET_02._Mini_ASP.Middlewares;
class LoggerMiddlware : IMiddleware
{
    public HttpHandler Next { get; set; }

    public void Handle(HttpListenerContext context)
    {
        Console.WriteLine(@$"{context.Request.HttpMethod}
{context.Request.RawUrl}
{context.Request.RemoteEndPoint}
");
        Next.Invoke(context);
    }
}
