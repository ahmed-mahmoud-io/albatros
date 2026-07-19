
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    public class ReviewsController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

     

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Property)
                .ToListAsync();

            return View(reviews);
        }

        
        // تفاصيل التقييم
      

        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
                return NotFound();

            return View(review);
        }

       
        // حذف التقييم
  

        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
                return NotFound();

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review != null)
            {
                _context.Reviews.Remove(review);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}