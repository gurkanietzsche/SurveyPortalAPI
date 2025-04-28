using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyPortalAPI.DTOs;
using SurveyPortalAPI.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = ApplicationRoles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserRepository userRepository, UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var userDTOs = users.Select(u => new UserDTO
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            CreatedDate = u.CreatedDate
        }).ToList();

        return Ok(userDTOs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            CreatedDate = user.CreatedDate
        });
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = true;
        await _userRepository.UpdateUserAsync(user);
        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = false;
        await _userRepository.UpdateUserAsync(user);
        return NoContent();
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AddUserToRole(string id, [FromBody] string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}/roles/{roleName}")]
    public async Task<IActionResult> RemoveUserFromRole(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}
