using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Data;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationDbContext _context;

        public NotificationController(NotificationDbContext context)
        {
            _context = context;
        }

        // 1. ≈—”«· ≈‘⁄«—
        [HttpPost]
        public async Task<IActionResult> SendNotification(Notification notification)
        {
            notification.SentAt = DateTime.UtcNow;
            notification.IsRead = false;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Notification sent successfully!" });
        }

        // 2. «” —Ã«⁄ Ã„Ì⁄ «·≈‘⁄«—« 
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _context.Notifications.ToListAsync();
            return Ok(notifications);
        }

        // 3. «” —Ã«⁄ ≈‘⁄«— „Õœœ
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Notification not found" });
            }
            return Ok(notification);
        }

        // 4.  ÕœÌÀ Õ«·… «·≈‘⁄«— ≈·Ï "„ﬁ—Ê¡"
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Notification not found" });
            }

            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification marked as read" });
        }
    }
}