using ASP_NET_20._TaskFlow_Refactored.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_20._TaskFlow_Refactored.Data;

public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions options) 
        : base(options)
    {}

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

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
                project.Property(p => p.OwnerId)
                    .IsRequired()
                    .HasMaxLength(450);
                project.HasOne(p => p.Owner)
                    .WithMany()
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
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
                task.Property(t => t.Priority)
                    .IsRequired();


                task.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            }                      
            );

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(
            refreshToken =>
            {
                refreshToken.HasKey(rt => rt.Id);
                refreshToken.HasIndex(rt => rt.JwtId).IsUnique();
                refreshToken.Property(rt=>rt.JwtId)
                    .IsRequired()
                    .HasMaxLength(64);
                refreshToken.Property(rt => rt.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

            }
            );

        // ProjectMember
        modelBuilder.Entity<ProjectMember>(
            member =>
            {
                member.HasKey(m => new { m.ProjectId, m.UserId });
                member.HasOne(m => m.Project)
                    .WithMany(p => p.Members)
                    .HasForeignKey(m => m.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
                member.HasOne(m => m.User)
                    .WithMany(u => u.ProjectMemberships)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                member.Property(m => m.UserId)
                    .HasMaxLength(450);
            });
    }

}
