using ASP_NET_02._CoR.Abstract;
using System.Text.RegularExpressions;

namespace ASP_NET_02._CoR.Concrete;
class PasswordChecker : BaseChecker
{
    public override bool Check(object request)
    {
        if (request is User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Password) &&
                new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}")
                .IsMatch(user.Password))
            {
                return Next.Check(request);
            }
        }
        return false;
    }
}
