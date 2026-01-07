using ASP_NET_02._CoR;
using ASP_NET_02._CoR.Concrete;

User user = new User("mr.13", "Salam12345", "zamanov@itstep.org");

var director = new CheckDirector();
Console.WriteLine(director.MakeUserChecker(user));
