using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace ASP_NET_06._Html_Helpers.HtmlHelpers;

public static class MyHtmlHelpers
{
    public static HtmlString ListFor(
        this IHtmlHelper helper,
        IEnumerable<object> items,
        string listTag = "ul",
        string color = "black",
        string fontSize = "16"
        )
    {
        var sb = new StringBuilder();
        sb.Append($"""
            <{listTag}
            style='color:{color};
            font-size:{fontSize}px;'>
            """);
        foreach ( var item in items)
        {
            sb.Append($"<li>{item}</li>");
        }
        sb.Append($"</{listTag}>");
        return new HtmlString( sb.ToString());
    }

    public static HtmlString MyForm(
        this IHtmlHelper helper,
        string buttonText = "Send",
        string method = "get",
        string action = "",
        string contriller = ""

        )
    {
        Random random = new Random();
        return new HtmlString($"""
            <form method="{method}" action="/{contriller}/{action}">
                <input type="hidden" value="{random.Next(1, 100)}" name="Id"/>
                <p>
                    <label for="Login">Login</label>
                </p>
                <p>
                    <input type="text" name="Login" />
                </p>
                <p>
                    <label for="Password">Password</label>
                </p>
                <p>
                    <input type="password" name="Password" />
                </p>
                <input type="submit" value="{buttonText}"/>
            </form>
            """);
    }
}
