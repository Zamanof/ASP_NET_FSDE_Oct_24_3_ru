namespace ASP_NET_02._CoR.Abstract;

interface IChecker
{
    public IChecker Next { get; set; }
    public bool Check(object request);
}
