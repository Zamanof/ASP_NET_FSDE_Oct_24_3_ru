namespace ASP_NET_06._Html_Helpers.Models;

public class User
{
    public string Login { get; set; } =string.Empty;
    public string Password { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Login}";
    }
}
