using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_23._TaskFlow_CQRS.Application.Common;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Identity;

namespace ASP_NET_23._TaskFlow_CQRS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class UserRolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public UserRolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
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
        if (user is null) return NotFound("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        }));
    }

    [HttpPost("{userId}/roles")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> AssignRole(string userId, [FromBody] AssignRoleDto assignRole)
    {
        var roleName = assignRole.Role.Trim();
        if (string.IsNullOrEmpty(roleName)) return BadRequest();
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return BadRequest();
        var result = await _userManager.AddToRoleAsync(user, assignRole.Role);
        if (!result.Succeeded) return BadRequest();
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        }));
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> RemoveRole(string userId, string roleName)
    {
        roleName = roleName.Trim();
        if (string.IsNullOrEmpty(roleName)) return BadRequest();
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return BadRequest();
        if (!await _userManager.IsInRoleAsync(user, roleName)) return BadRequest();
        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded) return BadRequest();
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        }));
    }
}
