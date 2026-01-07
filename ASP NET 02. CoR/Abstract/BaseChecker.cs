namespace ASP_NET_02._CoR.Abstract;

abstract class BaseChecker : IChecker
{
    public IChecker Next { get; set; }
    public abstract bool Check(object request);
}
