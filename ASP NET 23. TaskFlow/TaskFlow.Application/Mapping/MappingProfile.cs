using AutoMapper;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain;
using TaskStatus = TaskFlow.Domain.TaskStatus;

namespace TaskFlow.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectResponseDto>()
            .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count));

        CreateMap<CreateProjectDto, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tasks, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore());

        CreateMap<UpdateProjectDto, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore());

        CreateMap<TaskItem, TaskItemResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));

        CreateMap<CreateTaskItemDto, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TaskStatus.ToDo))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.Attachments, opt => opt.Ignore());

        CreateMap<UpdateTaskItemDto, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.Attachments, opt => opt.Ignore());
    }
}
