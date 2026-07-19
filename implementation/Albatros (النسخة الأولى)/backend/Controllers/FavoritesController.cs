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

        //==========================
        // View favorites
        //==========================

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId.Value)
                .Include(f => f.Property)
                    .ThenInclude(p => p.Images)
                .ToListAsync();

            return View(favorites);
        }

        //==========================
        // Add to favorites
        //==========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int propertyId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId.Value && f.PropertyId == propertyId);

            if (!exists)
            {
                var favorite = new Favorite
                {
                    UserId = userId.Value,
                    PropertyId = propertyId
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Properties", new { id = propertyId });
        }

        //==========================
        // Remove from favorites
        //==========================

        public async Task<IActionResult> Remove(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var favorite = await _context.Favorites.FindAsync(id);

            if (favorite == null || favorite.UserId != userId.Value)
                return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
