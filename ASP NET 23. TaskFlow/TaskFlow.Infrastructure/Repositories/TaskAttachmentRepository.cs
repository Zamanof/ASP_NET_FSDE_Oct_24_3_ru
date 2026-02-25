using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class TaskAttachmentRepository : ITaskAttachmentRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskAttachmentRepository(TaskFlowDbContext context) => _context = context;

    public async Task<TaskAttachment> AddAsync(TaskAttachment attachment)
    {
        _context.TaskAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }

    public async Task<TaskAttachment?> GetByIdAsync(int id) =>
        await _context.TaskAttachments.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<TaskAttachment?> GetByIdWithTaskItemAsync(int id) =>
        await _context.TaskAttachments.Include(a => a.TaskItem).FirstOrDefaultAsync(a => a.Id == id);

    public async Task RemoveAsync(TaskAttachment attachment)
    {
        _context.TaskAttachments.Remove(attachment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskAttachment attachment)
    {
        _context.TaskAttachments.Update(attachment);
        await _context.SaveChangesAsync();
    }
}
