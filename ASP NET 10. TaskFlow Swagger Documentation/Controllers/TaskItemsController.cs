using ASP_NET_10._TaskFlow_Swagger_Documentation.DTOs.TaskItem_DTOs;
using ASP_NET_10._TaskFlow_Swagger_Documentation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_10._TaskFlow_Swagger_Documentation.Controllers
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

        /// <summary>
        /// Retrieves all task items.
        /// </summary>
        /// <returns>List of all task items in the system</returns>
        /// <response code="200">Successfully retrieved the list of task items</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetAll()
        {
            var TaskItemResponseDtos = await _taskItemService.GetAllAsync();
            return Ok(TaskItemResponseDtos);
        }

        /// <summary>
        /// Retrieves a task item by its identifier.
        /// </summary>
        /// <param name="id">Task item identifier</param>
        /// <returns>Task item information</returns>
        /// <response code="200">Task item successfully found and returned</response>
        /// <response code="404">Task item with the specified identifier not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskItemResponseDto>> GetById(int id)
        {
            var TaskItemResponseDto = await _taskItemService.GetByIdAsync(id);
            if (TaskItemResponseDto is null)
                return NotFound($"TaskItemResponseDto with ID {id} not found");
            return Ok(TaskItemResponseDto);
        }

        /// <summary>
        /// Retrieves task items by project identifier.
        /// </summary>
        /// <param name="projectId">Project identifier</param>
        /// <returns>List of task items belonging to the specified project</returns>
        /// <response code="200">Task items successfully found and returned</response>
        /// <response code="404">Task items for the specified project not found</response>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetByProjectId(int projectId)
        {
            var taskItems = await _taskItemService.GetByProjectIdAsync(projectId);
            if (taskItems == null || !taskItems.Any())
                return NotFound($"Task items for project with ID {projectId} not found");
            return Ok(taskItems);
        }

        /// <summary>
        /// Creates a new task item.
        /// </summary>
        /// <param name="createRequest">Data for creating a task item</param>
        /// <returns>Created task item</returns>
        /// <response code="201">Task item successfully created</response>
        /// <response code="400">Invalid request data or validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Updates an existing task item.
        /// </summary>
        /// <param name="id">Task item identifier to update</param>
        /// <param name="updateRequest">Data for updating the task item</param>
        /// <returns>Updated task item</returns>
        /// <response code="200">Task item successfully updated</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Task item with the specified identifier not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskItemResponseDto?>> Update(int id, [FromBody] TaskItemUpdateRequest updateRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedTaskItemResponseDto = await _taskItemService.UpdateAsync(id, updateRequest);

            if (updatedTaskItemResponseDto is null)
                return NotFound($"TaskItemResponseDto with ID {id} not found");

            return Ok(updatedTaskItemResponseDto);
        }

        /// <summary>
        /// Deletes a task item by its identifier.
        /// </summary>
        /// <param name="id">Task item identifier to delete</param>
        /// <returns>Result of the delete operation</returns>
        /// <response code="204">Task item successfully deleted</response>
        /// <response code="404">Task item with the specified identifier not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var isDeleted = await _taskItemService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound($"TaskItemResponseDto with ID {id} not found");

            return NoContent();
        }
    }
}
