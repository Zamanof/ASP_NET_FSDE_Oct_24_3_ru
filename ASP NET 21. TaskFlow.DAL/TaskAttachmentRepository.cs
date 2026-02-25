using ASP_NET_21._TaskFlow.Data;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_21._TaskFlow.DAL;

public class TaskAttachmentRepository : ITaskAttachmentRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskAttachmentRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<TaskAttachment> AddAsync(TaskAttachment attachment)
    {
        _context.TaskAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }

    public async Task<TaskAttachment?> GetByIdAsync(int id)
    {
        return await _context.TaskAttachments.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<TaskAttachment?> GetByIdWithTaskItemAsync(int id)
    {
        return await _context.TaskAttachments
            .Include(a => a.TaskItem)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

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
