using ASP_NET_02._CoR.Abstract;
using System.Text.RegularExpressions;

namespace ASP_NET_02._CoR.Concrete;

class EmailChecker : BaseChecker
{
    public override bool Check(object request)
    {
        if (request is User user)
        {
            return !string.IsNullOrWhiteSpace(user.Email) &&
                new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")
                .IsMatch(user.Email);
        }
        return false;
    }
}
