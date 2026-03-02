using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Commands;

public record DeleteProjectComand(int Id): IRequest<bool>;
