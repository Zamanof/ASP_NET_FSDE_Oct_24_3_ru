using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_01._Razor_page_example.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

}
