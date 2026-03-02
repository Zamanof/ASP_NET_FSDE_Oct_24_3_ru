using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Commands;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Application.Mapping;
using ASP_NET_23._TaskFlow_CQRS.Domain;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace ASP_NET_23._TaskFlow_CQRS_Unit_Test.Handlers;

public class CreateProjectCommandHandlerTests
{
    private static readonly IMapper Mapper = new MapperConfiguration(
        config => config.AddProfile<MappingProfile>()).CreateMapper();
    [Fact]
    public async Task Handle_ValidCommand_CallsAddAsyncAndReturnsDto()
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

        projectRepo.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync(project);

        var handler = new CreateProjectCommandHandler(projectRepo.Object, Mapper);
        var command = new CreateProjectCommand(new CreateProjectDto { Name = "proj1", Description = "Desc" }, "user1");

        // Act
        var result =await  handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("proj1");
        result.OwnerId.Should().Be("user1");
        projectRepo.Verify(r => r.AddAsync(It.Is<Project>(p => p.OwnerId == "user1")), Times.Once);

    }
}
