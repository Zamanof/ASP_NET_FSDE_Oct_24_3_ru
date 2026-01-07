using ASP_NET_02._Mini_ASP.Interfaces;
using System.Net;

namespace ASP_NET_02._Mini_ASP.Middlewares;
class StaticFilesMiddlware : IMiddleware
{
    public HttpHandler Next { get; set; }

    public void Handle(HttpListenerContext context)
    {
        if (Path.HasExtension(context.Request.RawUrl))
        {
            try
            {
                var fileName = context.Request.RawUrl.Substring(1);
                var path = $@"..\..\..\wwwroot\{fileName}";
                var bytes = File.ReadAllBytes(path);
                if (Path.GetExtension(path) == ".html")
                {
                    context.Response.AddHeader("Content-Type", "text/html");
                }
                else if (Path.GetExtension(path) == ".png")
                {
                    context.Response.AddHeader("Content-Type", "image/png");
                }
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = "File Not Found";
                var path = $@"..\..\..\wwwroot\404.html";
                var bytes = File.ReadAllBytes(path);
                context.Response.AddHeader("Content-Type", "text/html");
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
        }
        else
        {
            Next.Invoke(context);
        }
        context.Response.Close();
    }
}
