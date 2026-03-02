using ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Commands;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace ASP_NET_23._TaskFlow_CQRS_Unit_Test.Handlers;

public class DeleteProjectComandHandlerTests
{
    [Fact]
    public async Task Handle_ProjectExists_RemoveAndReturnsTrue()
    {
        // Arrange
        var projectRepo = new Mock<IProjectRepository>();
        var project = new Project
        {
            Id = 1,
            Name = "proj1",
            OwnerId = "user1",
            CreatedAt = DateTimeOffset.UtcNow
        };

        projectRepo.Setup(r => r.FindAsync(1)).ReturnsAsync(project);

        var handler = new DeleteProjectComandHandler(projectRepo.Object);
        var command = new DeleteProjectComand(1);

        // Act

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        projectRepo.Verify(r => r.RemoveAsync(project), Times.Once);
    }


    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFalse()
    {
        // Arrange
        var projectRepo = new Mock<IProjectRepository>();

        projectRepo.Setup(r => r.FindAsync(999)).ReturnsAsync((Project?)null);

        var handler = new DeleteProjectComandHandler(projectRepo.Object);
        var command = new DeleteProjectComand(99);

        // Act

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        projectRepo.Verify(r => r.RemoveAsync(It.IsAny<Project>()), Times.Never);
    }
}
