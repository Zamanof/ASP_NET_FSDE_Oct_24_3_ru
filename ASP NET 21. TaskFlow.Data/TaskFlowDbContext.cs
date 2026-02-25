using ASP_NET_21._TaskFlow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_21._TaskFlow.Data;

public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions options)
        : base(options)
    { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project (as in ASP NET 20. TaskFlow Files)
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.OwnerId).IsRequired().HasMaxLength(450);
            entity.HasOne(p => p.Owner).WithMany().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });

        // TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).HasMaxLength(1000);
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.Property(t => t.Status).IsRequired();
            entity.Property(t => t.Priority).IsRequired();
            entity.HasOne(t => t.Project).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.JwtId).IsUnique();
            entity.Property(rt => rt.JwtId).IsRequired().HasMaxLength(64);
            entity.Property(rt => rt.UserId).IsRequired().HasMaxLength(450);
        });

        // ProjectMember
        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(m => new { m.ProjectId, m.UserId });
            entity.HasOne(m => m.Project).WithMany(p => p.Members).HasForeignKey(m => m.ProjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(m => m.User).WithMany(u => u.ProjectMemberships).HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(m => m.UserId).HasMaxLength(450);
        });

        // TaskAttachment (as in ASP NET 20: StoredFileName 200, ContentType 100)
        modelBuilder.Entity<TaskAttachment>(entity =>
        {
            entity.HasKey(ta => ta.Id);
            entity.Property(ta => ta.OriginalFileName).IsRequired().HasMaxLength(500);
            entity.Property(ta => ta.StoredFileName).IsRequired().HasMaxLength(200);
            entity.Property(ta => ta.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(ta => ta.UploadedByUserId).IsRequired().HasMaxLength(450);
            entity.HasOne(ta => ta.TaskItem).WithMany(t => t.Attachments).HasForeignKey(ta => ta.TaskItemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ta => ta.UploadedByUser).WithMany().HasForeignKey(ta => ta.UploadedByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
