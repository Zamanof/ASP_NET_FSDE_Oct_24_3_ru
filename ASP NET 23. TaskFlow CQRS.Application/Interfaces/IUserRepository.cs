using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<AvailableUserDto>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds);
}
