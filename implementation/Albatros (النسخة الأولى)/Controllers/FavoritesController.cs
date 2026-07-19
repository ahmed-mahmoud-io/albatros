using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Property)
                    .ThenInclude(p => p.Images)
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int propertyId)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return RedirectToAction("Login", "Account");
            int userId = sessionUserId.Value;

            bool exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.PropertyId == propertyId);

            if (!exists)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    PropertyId = propertyId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Properties", new { id = propertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null) return NotFound();
            if (favorite.UserId != userId)
                return Forbid();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int propertyId)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return RedirectToAction("Login", "Account");
            int userId = sessionUserId.Value;

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    PropertyId = propertyId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Properties", new { id = propertyId });
        }
    }
}