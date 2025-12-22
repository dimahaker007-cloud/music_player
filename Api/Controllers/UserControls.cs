using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using models;

namespace Api.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMemoryCache _cache;

    public UsersController(IUserService userService, IMemoryCache cache)
    {
        _userService = userService;
        _cache = cache;
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
    public async Task<ActionResult<UserLoginDto>> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => 
                u.name == loginDto.name && u.password == loginDto.password);
            
            if (user == null)
                return Unauthorized("Invalid credentials");
                
            var userDto = new UserLoginDto()
            {
                password = user.password,
                name = user.name
            };
            
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpPost]
    public async Task<ActionResult> CreateUser(
        [FromBody] User userReq, 
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
    {
        var requestId = HttpContext.Items["RequestId"]?.ToString() ?? "unknown";

        if (string.IsNullOrEmpty(idempotencyKey))
            return BadRequest(new ApiErrorResponse { Error = "Missing Idempotency-Key", RequestId = requestId });

        try
        {
            // --- ІДЕМПОТЕНТНІСТЬ ---
            if (_cache.TryGetValue(idempotencyKey, out UserDto? cachedUser))
            {
                Response.Headers.Add("X-Idempotency-Cache", "HIT");
                return Ok(cachedUser);
            }

            var user = new User { name = userReq.name, password = userReq.password };
            var userId = await _userService.AddUserAsync(user);
            
            if (userId <= 0) throw new Exception("Database error");

            var userDto = new UserDto { id = user.id, name = user.name };

            // Зберігаємо результат у кеш
            _cache.Set(idempotencyKey, userDto, TimeSpan.FromHours(1));

            return CreatedAtAction(nameof(GetUserById), new { id = user.id }, userDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiErrorResponse { 
                Error = "Internal Server Error", 
                Details = ex.Message, 
                RequestId = requestId 
            });
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Перевірка, чи існує користувач
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            // Перевірка, чи існує користувач
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found");

            var deleted = await _userService.DeleteUserAsync(id);
                
            if (!deleted)
                return StatusCode(500, "Failed to delete user");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
}