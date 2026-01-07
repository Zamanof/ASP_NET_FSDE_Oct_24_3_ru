using ASP_NET_02._Mini_ASP;

WebHost host = new(27001);
host.UseStartup<Startup>();
host.Run();
