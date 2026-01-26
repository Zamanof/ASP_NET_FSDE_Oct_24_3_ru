using ASP_NET_09._TaskFlow_DTO.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_09._TaskFlow_DTO.Data;

public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions options) 
        : base(options)
    {}

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Fluent API
        // Project
        modelBuilder.Entity<Project>(
            project =>
            {
                project.HasKey(p => p.Id);
                project.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                project.Property(p => p.Description)
                    .HasMaxLength(1000);
                project.Property(p => p.CreatedAt)
                    .IsRequired();
            }
            );

        // TaskItem
        modelBuilder.Entity<TaskItem>(
            task=>
            {
                task.HasKey(t => t.Id);
                task.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                task.Property(t => t.Description)
                    .HasMaxLength(1000);
                task.Property(t => t.Status)
                    .IsRequired();
                task.Property(t => t.CreatedAt)
                    .IsRequired();


                task.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            }                      
            );

    }

}
