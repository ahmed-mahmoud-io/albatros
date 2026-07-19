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

        // عرض كل التقييمات
        

        public async Task<IActionResult> Index(int propertyId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .Include(r => r.User)
                .ToListAsync();

            ViewBag.PropertyId = propertyId;

            return View(reviews);
        }

       
        // إضافة تقييم
       

        public IActionResult Create(int propertyId)
        {
            Review review = new Review
            {
                PropertyId = propertyId
            };

            return View(review);
        }

       
        // حفظ التقييم
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (!ModelState.IsValid)
                return View(review);

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            review.UserId = userId.Value;
            review.CreatedAt = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Properties",
                new { id = review.PropertyId });
        }

       
        // تعديل
        

        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            if (id != review.ReviewId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(review);

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index),
                new { propertyId = review.PropertyId });
        }

        // حذف
   

        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
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

            if (review == null)
                return NotFound();

            int propertyId = review.PropertyId;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index),
                new { propertyId });
        }
    }
}