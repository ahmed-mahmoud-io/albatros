using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    [Area("Admin")]
    public class AdminPropertiesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPropertiesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var properties = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(properties);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var property = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null) return NotFound();
            return View(property);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            if (id != property.PropertyId) return NotFound();

            var existing = await _context.Properties.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = property.Title;
            existing.Description = property.Description;
            existing.Price = property.Price;
            existing.City = property.City;
            existing.Bedrooms = property.Bedrooms;
            existing.Bathrooms = property.Bathrooms;
            existing.Area = property.Area;
            existing.Status = property.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            property.Status = PropertyStatus.Approved;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            property.Status = PropertyStatus.Rejected;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null) return NotFound();
            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

