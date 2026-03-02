using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Commands;

public class DeleteProjectComandHandler : IRequestHandler<DeleteProjectComand, bool>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectComandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<bool> Handle(DeleteProjectComand request, CancellationToken cancellationToken)
    {
        var project =await _projectRepository.FindAsync(request.Id);
        
        if (project is null) return false;

        await _projectRepository.RemoveAsync(project);

        return true;
    }
}
