using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Common;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.TaskItem_DTOs;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Controllers
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
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetAll()
        {
            var taskItems = await _taskItemService.GetAllAsync();

            return Ok(
                ApiResponse<IEnumerable<TaskItemResponseDto>>
                    .SuccessResponse(taskItems, "Task items retrieved successfully")
            );
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<IEnumerable<TaskItemResponseDto>>>>> GetPaged([FromQuery]TaskItemQueryParams queryParams)
        {
            var result = await _taskItemService.GetPagedAsync(queryParams);

            return Ok(
                ApiResponse<PagedResult<TaskItemResponseDto>>
                    .SuccessResponse(result, "Task items retrieved successfully")
            );
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
        public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetById(int id)
        {
            var taskItem = await _taskItemService.GetByIdAsync(id);

            if (taskItem is null)
                return NotFound(
                    ApiResponse<TaskItemResponseDto>
                        .ErrorResponse($"Task item with ID {id} not found")
                );

            return Ok(
                ApiResponse<TaskItemResponseDto>
                    .SuccessResponse(taskItem, "Task item found")
            );
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
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetByProjectId(int projectId)
        {
            var taskItems = await _taskItemService.GetByProjectIdAsync(projectId);

            if (taskItems == null || !taskItems.Any())
                return NotFound(
                    ApiResponse<IEnumerable<TaskItemResponseDto>>
                        .ErrorResponse($"Task items for project with ID {projectId} not found")
                );

            return Ok(
                ApiResponse<IEnumerable<TaskItemResponseDto>>
                    .SuccessResponse(taskItems, $"Task items for project {projectId} retrieved successfully")
            );
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
        public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Create([FromBody] TaskItemCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    ApiResponse<TaskItemResponseDto>
                        .ErrorResponse("Invalid request data")
                );


            var createdTaskItem = await _taskItemService.CreateAsync(createRequest);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTaskItem.Id },
                ApiResponse<TaskItemResponseDto>
                    .SuccessResponse(createdTaskItem, "Task item created successfully")
            );
            //try
            //{
            //    var createdTaskItem = await _taskItemService.CreateAsync(createRequest);

            //    return CreatedAtAction(
            //        nameof(GetById),
            //        new { id = createdTaskItem.Id },
            //        ApiResponse<TaskItemResponseDto>
            //            .SuccessResponse(createdTaskItem, "Task item created successfully")
            //    );
            //}
            //catch (ArgumentException ex)
            //{
            //    return BadRequest(
            //        ApiResponse<TaskItemResponseDto>
            //            .ErrorResponse(ex.Message)
            //    );
            //}
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
        public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Update(int id, [FromBody] TaskItemUpdateRequest updateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    ApiResponse<TaskItemResponseDto>
                        .ErrorResponse("Invalid request data")
                );

            var updatedTaskItem = await _taskItemService.UpdateAsync(id, updateRequest);

            if (updatedTaskItem is null)
                return NotFound(
                    ApiResponse<TaskItemResponseDto>
                        .ErrorResponse($"Task item with ID {id} not found")
                );

            return Ok(
                ApiResponse<TaskItemResponseDto>
                    .SuccessResponse(updatedTaskItem, "Task item updated successfully")
            );
        }

        /// <summary>
        /// Deletes a task item by its identifier.
        /// </summary>
        /// <param name="id">Task item identifier to delete</param>
        /// <returns>Result of the delete operation</returns>
        /// <response code="200">Task item successfully deleted</response>
        /// <response code="404">Task item with the specified identifier not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var isDeleted = await _taskItemService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound(
                    ApiResponse<object>
                        .ErrorResponse($"Task item with ID {id} not found")
                );

            return Ok(
                ApiResponse<object>
                    .SuccessResponse(null, "Task item deleted successfully")
            );
        }
    }
}
