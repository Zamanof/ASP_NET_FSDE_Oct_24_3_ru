using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using AutoMapper;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Queries;

public class GetProjectByIdQueryHandler: IRequestHandler<GetProjectByIdQuery, ProjectResponseDto>
{
    private readonly IProjectService _projectService;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(IProjectService projectService, IMapper mapper)
    {
        _projectService = projectService;
        _mapper = mapper;
    }

    public async Task<ProjectResponseDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(request.Id);
        return _mapper.Map<ProjectResponseDto>(project);
    }
}
