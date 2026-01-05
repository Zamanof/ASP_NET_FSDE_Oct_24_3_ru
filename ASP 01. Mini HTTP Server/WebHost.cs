using System.Net;

namespace ASP_01._Mini_HTTP_Server;
class WebHost
{
    int port;
    string pathBase = @"../../../";
    HttpListener httpListener;

    public WebHost(int port)
    {
        this.port = port;
    }

    public void Run()
    {
        httpListener = new HttpListener();
        httpListener.Prefixes.Add(@$"http://localhost:{port}/");
        httpListener.Start();
        Console.WriteLine($"Http Server started on {port}");
        while (true)
        {
            var context = httpListener.GetContext();
            Task.Run(()=>HandleRequest(context));
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var url = context.Request.RawUrl;
        var path = @$"{pathBase}{url.Split("/").Last()}";
        var response = context.Response;
        StreamWriter streamWriter = new(response.OutputStream);
        try
        {
            var src = File.ReadAllText(path);
            streamWriter.WriteLine(src);
        }
        catch (Exception)
        {
            var src = File.ReadAllText(@$"{pathBase}404.html");
            streamWriter.WriteLine(src);
        }
        finally
        {
            streamWriter.Close();
        }
    }
}
