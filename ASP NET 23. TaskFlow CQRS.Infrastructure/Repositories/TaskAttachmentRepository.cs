using Microsoft.EntityFrameworkCore;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Data;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Repositories;

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
