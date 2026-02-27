using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks;

public record GetTaskByIdQuery(int Id): IRequest<TaskItemResponseDto?>;

class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItemResponseDto?>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }

    public async Task<TaskItemResponseDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskItemRepository.GetByIdWithProjectAsync(request.Id);
        return _mapper.Map<TaskItemResponseDto?>(task);
    }
}