using TaskFlow.Domain;

namespace TaskFlow.Application.Interfaces;

public interface ITaskAttachmentRepository
{
    Task<TaskAttachment?> GetByIdAsync(int id);
    Task<TaskAttachment?> GetByIdWithTaskItemAsync(int id);
    Task<TaskAttachment> AddAsync(TaskAttachment attachment);
    Task UpdateAsync(TaskAttachment attachment);
    Task RemoveAsync(TaskAttachment attachment);
}
