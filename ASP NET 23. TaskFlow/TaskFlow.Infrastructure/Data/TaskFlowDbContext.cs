using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Data;

public class TaskFlowDbContext : IdentityDbContext<AppUser>
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // As in ASP NET 20. TaskFlow Files: Description optional, Fluent API aligned
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.OwnerId).IsRequired().HasMaxLength(450);
            entity.HasOne<AppUser>().WithMany().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });

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

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.JwtId).IsUnique();
            entity.Property(e => e.JwtId).IsRequired().HasMaxLength(64);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.UserId });
            entity.HasOne(e => e.Project).WithMany(p => p.Members).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        // As in ASP NET 20: StoredFileName 200, ContentType 100
        modelBuilder.Entity<TaskAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.StoredFileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UploadedByUserId).IsRequired().HasMaxLength(450);
            entity.HasOne(e => e.TaskItem).WithMany(t => t.Attachments).HasForeignKey(e => e.TaskItemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UploadedByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
