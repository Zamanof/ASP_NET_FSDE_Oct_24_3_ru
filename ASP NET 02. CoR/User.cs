namespace ASP_NET_02._CoR;

class User
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }

    public User(string? userName, string? password, string? email)
    {
        UserName = userName;
        Password = password;
        Email = email;
    }
}
