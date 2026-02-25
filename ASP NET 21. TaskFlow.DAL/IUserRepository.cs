using ASP_NET_21._TaskFlow.Models;

namespace ASP_NET_21._TaskFlow.DAL;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds);
}
