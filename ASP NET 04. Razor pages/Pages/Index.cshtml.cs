using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_04._Razor_pages.Pages;

public class IndexModel : PageModel
{
    public void OnGet(Person person)
    {
        ViewData["Name"] = person.Name;
        ViewData["Age"] = person.Age;
    }

    public string Foo(string str) => str.Replace('a', 'u');
}
