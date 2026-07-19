using Albatros.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    public class UsersController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // All users
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationUsers.ToListAsync());
        }

        // User details
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // Delete user
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
