using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int propertyId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var review = new Review { PropertyId = propertyId };
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            review.UserId = userId.Value;
            review.CreatedAt = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Properties", new { id = review.PropertyId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            if (review.UserId != userId) return Forbid();

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var existing = await _context.Reviews.FindAsync(id);
            if (existing == null) return NotFound();
            if (existing.UserId != userId) return Forbid();

            existing.Rating = review.Rating;
            existing.Comment = review.Comment;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Properties", new { id = existing.PropertyId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null) return NotFound();
            if (review.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            if (review.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            int propertyId = review.PropertyId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Properties", new { id = propertyId });
        }
    }
}