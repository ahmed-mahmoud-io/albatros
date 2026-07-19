using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            var users = await _context.ApplicationUsers.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            if (id == null) return NotFound();

            var user = await _context.ApplicationUsers
                .Include(u => u.Properties)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            if (id == null) return NotFound();

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                _context.ApplicationUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}