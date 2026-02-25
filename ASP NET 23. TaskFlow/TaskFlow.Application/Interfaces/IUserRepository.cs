using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<AvailableUserDto>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds);
}
