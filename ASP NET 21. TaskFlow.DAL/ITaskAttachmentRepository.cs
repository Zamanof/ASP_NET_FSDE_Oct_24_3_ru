using ASP_NET_21._TaskFlow.Models;

namespace ASP_NET_21._TaskFlow.DAL;

public interface ITaskAttachmentRepository
{
    Task<TaskAttachment?> GetByIdAsync(int id);
    Task<TaskAttachment?> GetByIdWithTaskItemAsync(int id);
    Task<TaskAttachment> AddAsync(TaskAttachment attachment);
    Task UpdateAsync(TaskAttachment attachment);
    Task RemoveAsync(TaskAttachment attachment);
}
