using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Queries;
public record GetAllProjectsQuery(string UserId, IList<string> Roles) 
    : IRequest<IEnumerable<ProjectResponseDto>>;