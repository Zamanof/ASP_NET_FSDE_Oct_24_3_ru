using ASP_NET_09._TaskFlow_DTO.DTOs.TaskItem_DTOs;
using ASP_NET_09._TaskFlow_DTO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_09._TaskFlow_DTO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;

        public TaskItemsController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetAll()
        {
            var TaskItemResponseDtos = await _taskItemService.GetAllAsync();
            return Ok(TaskItemResponseDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemResponseDto>> GetById(int id)
        {
            var TaskItemResponseDto = await _taskItemService.GetByIdAsync(id);
            if (TaskItemResponseDto is null)
                return NotFound($"TaskItemResponseDto with ID {id} not found");
            return Ok(TaskItemResponseDto);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<TaskItemResponseDto>> GetByProjectId(int projectId)
        {
            var TaskItemResponseDto = await _taskItemService.GetByIdAsync(projectId);
            if (TaskItemResponseDto is null)
                return NotFound($"TaskItemResponseDto with ID {projectId} not found");
            return Ok(TaskItemResponseDto);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemResponseDto>> Create([FromBody] TaskItemCreateRequest createRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var createdTaskItemResponseDto = await _taskItemService.CreateAsync(createRequest);
                return CreatedAtAction(
                nameof(GetById),
                new { id = createdTaskItemResponseDto.Id },
                createdTaskItemResponseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }



        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItemResponseDto?>> Update(int id, [FromBody] TaskItemUpdateRequest updateRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedTaskItemResponseDto = await _taskItemService.UpdateAsync(id, updateRequest);

            if (updatedTaskItemResponseDto is null)
                return NotFound($"TaskItemResponseDto with ID {id} not found");

            return Ok(updatedTaskItemResponseDto);
        }

        [HttpDelete("{id}")]
        // base_path/id
        public async Task<ActionResult> Delete(int id)
        {
            var isDeleted = await _taskItemService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound($"TaskItemResponseDto with ID {id} not found");

            return NoContent();
        }
    }
}
