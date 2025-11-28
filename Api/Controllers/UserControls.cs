using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            
            var userDtos = users.Select(u => new UserDto
            {
                id = u.id,
                name = u.name
            });
            
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
                return NotFound();
                
            var userDto = new UserDto
            {
                id = user.id,
                name = user.name
            };
            
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/users/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => 
                u.name == loginDto.name && u.password == loginDto.password);
            
            if (user == null)
                return Unauthorized("Invalid credentials");
                
            var userDto = new UserDto
            {
                id = user.id,
                name = user.name
            };
            
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
}