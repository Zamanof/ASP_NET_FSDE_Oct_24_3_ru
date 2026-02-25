using ASP_NET_21._TaskFlow.BLL.Common;
using ASP_NET_21._TaskFlow.BLL.DTOs;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_21._TaskFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class UserRolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly string[] AllowedRoles = new[] { "Admin", "Manager", "User" };

    public UserRolesController(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserWithRolesDto>>>> GetAll()
    {
        var users = _userManager.Users.OrderBy(u => u.Email).ToList();

        var dtos = new List<UserWithRolesDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            dtos.Add(new UserWithRolesDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            });
        }

        return Ok(ApiResponse<IEnumerable<UserWithRolesDto>>.SuccessResponse(dtos));
    }

    [HttpGet("{userId}/roles")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> GetRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        var userWithRole = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(userWithRole));
    }

    [HttpPost("{userId}/roles")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>>
        AssignRole(string userId, [FromBody] AssignRoleDto assignRole)
    {
        var roleName = assignRole.Role.Trim();

        if (roleName is null)
            return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return BadRequest();

        var result = await _userManager.AddToRoleAsync(user, assignRole.Role);

        var roles = await _userManager.GetRolesAsync(user);

        var assignedRoleUser = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };
        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(assignedRoleUser));
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> 
        RemoveRole(string userId, string roleName)
    {
        roleName = roleName.Trim();

        if (string.IsNullOrEmpty(roleName))
            return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);

        if(user is null)
            return BadRequest();

        if (!await _userManager.IsInRoleAsync(user, roleName))
            return BadRequest();

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if(!result.Succeeded)
            return BadRequest();

        var roles = await _userManager.GetRolesAsync(user);

        var removedRoleUser = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(removedRoleUser));
    }
}
