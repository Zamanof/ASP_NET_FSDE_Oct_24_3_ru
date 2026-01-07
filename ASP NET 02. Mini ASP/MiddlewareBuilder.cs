namespace ASP_NET_02._Mini_ASP.Interfaces;

class MiddlewareBuilder
{
    private Stack<Type> middlewares = new();
    public void Use<T>() where T: IMiddleware
    {
        middlewares.Push(typeof(T));
    }

    public HttpHandler Build()
    {
        HttpHandler handler = context => context.Response.Close();
        while (middlewares.Count != 0)
        {
            var middleware = middlewares.Pop();
            IMiddleware middleWare = Activator.CreateInstance(middleware) as IMiddleware;
            middleWare.Next = handler;
            handler = middleWare.Handle;
        }
        return handler;

    }
}