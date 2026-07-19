using Albatros.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var notifs = await _context.Notifications
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifs);
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif != null)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var userNotifs = await _context.Notifications
                .Where(n => n.UserId == userId.Value)
                .ToListAsync();

            _context.Notifications.RemoveRange(userNotifs);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
