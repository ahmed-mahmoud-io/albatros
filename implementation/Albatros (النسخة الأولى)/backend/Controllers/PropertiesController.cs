using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly AppDbContext _context;

        public PropertiesController(AppDbContext context)
        {
            _context = context;
        }

        //=========================
        // Index
        //=========================

        public async Task<IActionResult> Index(string search, string city)
        {
            var properties = _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                properties = properties.Where(p =>
                    p.Title.Contains(search) ||
                    p.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(city))
            {
                properties = properties.Where(p => p.City == city);
            }

            return View(await properties.ToListAsync());
        }

        //=========================
        // Details
        //=========================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound();

            return View(property);
        }

        //=========================
        // Create
        //=========================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property)
        {
            if (!ModelState.IsValid)
                return View(property);

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            property.UserId = userId.Value;
            property.CreatedAt = DateTime.Now;

            _context.Properties.Add(property);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // Edit
        //=========================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties.FindAsync(id);

            if (property == null)
                return NotFound();

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.PropertyId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(property);

            _context.Properties.Update(property);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // Delete
        //=========================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound();

            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties.FindAsync(id);

            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // My Properties
        //=========================

        public async Task<IActionResult> MyProperties()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var properties = await _context.Properties
                .Where(p => p.UserId == userId.Value)
                .Include(p => p.Images)
                .ToListAsync();

            return View(properties);
        }

        //=========================
        // Search
        //=========================

        public async Task<IActionResult> Search(string keyword)
        {
            var result = await _context.Properties
                .Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Description.Contains(keyword) ||
                    p.City.Contains(keyword))
                .Include(p => p.Images)
                .ToListAsync();

            return View("Index", result);
        }

        //=========================
        // Filter
        //=========================

        public async Task<IActionResult> Filter(string city, decimal? minPrice, decimal? maxPrice, int? bedrooms)
        {
            var properties = _context.Properties
                .Include(p => p.Images)
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
                properties = properties.Where(p => p.City == city);

            if (minPrice.HasValue)
                properties = properties.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                properties = properties.Where(p => p.Price <= maxPrice.Value);

            if (bedrooms.HasValue)
                properties = properties.Where(p => p.Bedrooms == bedrooms.Value);

            return View("Index", await properties.ToListAsync());
        }

        //=========================
        // Helper
        //=========================

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.PropertyId == id);
        }

        private async Task<string?> UploadImage(IFormFile file)
        {
            if (file == null)
                return null;

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }
    }
}