using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Data;
using UserService.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly MessageBusClient _messageBusClient;

        public UserController(UserDbContext context)
        {
            _context = context;
            _messageBusClient = new MessageBusClient();
        }

        // 1.  ”ÃÌ· «·„” Œœ„ «·ÃœÌœ
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // ≈—”«· —”«·… ≈·Ï RabbitMQ
            var userCreatedDto = new UserCreatedDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email
            };
            _messageBusClient.PublishNewUser(userCreatedDto);
            return Ok(new { message = "User registered successfully!" });
        }

        // 2.  ”ÃÌ· «·œŒÊ·
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            return Ok(new { message = "Login successful", user });
        }

        // 3.  ⁄œÌ· «·„·› «·‘Œ’Ì ··„” Œœ„
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password; // ÌÃ»  ‘›Ì— ﬂ·„… «·„—Ê—
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully!" });
        }
    }
}
