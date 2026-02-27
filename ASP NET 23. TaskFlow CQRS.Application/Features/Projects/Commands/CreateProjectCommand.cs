using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Commands;



public record CreateProjectCommand(CreateProjectDto CreateProjectDto, string OwnerId): IRequest<ProjectResponseDto>;
