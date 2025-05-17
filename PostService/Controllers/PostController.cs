using Microsoft.AspNetCore.Mvc;
using PostService.Models;
using PostService.Data;
using Microsoft.EntityFrameworkCore;
using PostService.Services;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbContext _context;
        private readonly MessageBusClient _messageBusClient;
        public PostController(PostDbContext context)
        {
            _context = context;
            _messageBusClient = new MessageBusClient();
        }

        // 1. ≈‰‘«¡ „‰‘Ê— ÃœÌœ
        [HttpPost]
        public async Task<IActionResult> CreatePost(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            // ≈—”«· —”«·… ≈·Ï RabbitMQ
            var postCreatedDto = new PostCreatedDto
            {
                PostId = post.Id,
                Content = post.Content,
                UserId = post.UserId
            };
            _messageBusClient.PublishNewPost(postCreatedDto);
            return Ok(new { message = "Post created successfully!" });
        }

        // 2. «” —Ã«⁄ Ã„Ì⁄ «·„‰‘Ê—« 
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return Ok(posts);
        }

        // 3. «” —Ã«⁄ „‰‘Ê— „Õœœ
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }
            return Ok(post);
        }
    }
}