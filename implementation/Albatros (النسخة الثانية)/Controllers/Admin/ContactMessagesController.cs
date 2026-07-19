using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    [Area("Admin")]
    public class ContactMessagesController : Controller
    {
        private readonly AppDbContext _context;

        public ContactMessagesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View(messages);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();

            if (!msg.IsRead)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(msg);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg != null)
            {
                _context.ContactMessages.Remove(msg);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
