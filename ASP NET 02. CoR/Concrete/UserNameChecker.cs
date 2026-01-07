using ASP_NET_02._CoR.Abstract;

namespace ASP_NET_02._CoR.Concrete;

class UserNameChecker : BaseChecker
{
    public override bool Check(object request)
    {
        if (request is User user)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                return Next.Check(request);
            }
        }
        return false;
    }
}
