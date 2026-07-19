using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PropertiesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // عرض جميع العقارات المعتمدة
        public async Task<IActionResult> Index(string? search, string? city, decimal? minPrice, decimal? maxPrice, int? bedrooms, string? listingType)
        {
            var query = _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .Where(p => p.Status == PropertyStatus.Approved)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (bedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= bedrooms.Value);

            if (!string.IsNullOrEmpty(listingType) && listingType != "all")
            {
                var type = listingType == "sale" ? ListingType.Sale : ListingType.Rent;
                query = query.Where(p => p.ListingType == type);
            }

            ViewBag.Cities = await _context.Properties
                .Where(p => p.Status == PropertyStatus.Approved)
                .Select(p => p.City)
                .Distinct()
                .ToListAsync();

            ViewBag.CurrentCity = city;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentBedrooms = bedrooms;
            ViewBag.CurrentListingType = listingType;

            // Pass user's favorited property IDs for heart icon
            ViewBag.FavoritedIds = new HashSet<int>();
            var sid = HttpContext.Session.GetInt32("UserId");
            if (sid != null)
            {
                var ids = await _context.Favorites
                    .Where(f => f.UserId == sid.Value)
                    .Select(f => f.PropertyId)
                    .ToListAsync();
                ViewBag.FavoritedIds = new HashSet<int>(ids);
            }

            var properties = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            var villa21 = properties.FirstOrDefault(p => p.Title.Contains("Villa in Riyadh 21"));
            if (villa21 != null)
            {
                properties.Remove(villa21);
                properties.Add(villa21);
            }

            return View(properties);
        }

        // تفاصيل العقار
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.IsFavorited = false;
            if (userId != null)
            {
                ViewBag.IsFavorited = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.PropertyId == id);
            }

            ViewBag.AverageRating = property.Reviews != null && property.Reviews.Any()
                ? Math.Round(property.Reviews.Average(r => r.Rating), 1)
                : 0;

            ViewBag.RelatedProperties = await _context.Properties
                .Include(p => p.Images)
                .Where(p => p.City == property.City && p.PropertyId != id && p.Status == PropertyStatus.Approved)
                .Take(3)
                .ToListAsync();

            return View(property);
        }

        // إنشاء عقار
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("Role");
            if (role != "Owner" && role != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            ViewBag.Cities = new List<string> { "Riyadh", "Jeddah", "Mecca", "Dammam", "Khobar" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, List<IFormFile> images)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            property.UserId = userId.Value;
            property.CreatedAt = DateTime.Now;
            property.Status = PropertyStatus.Pending;

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Upload images
            if (images != null && images.Count > 0)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    var imageUrl = await UploadImage(images[i]);
                    if (imageUrl != null)
                    {
                        _context.PropertyImages.Add(new PropertyImage
                        {
                            PropertyId = property.PropertyId,
                            ImageUrl = imageUrl
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyProperties));
        }

        // تعديل عقار
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null) return NotFound();
            if (property.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            ViewBag.Cities = new List<string> { "Riyadh", "Jeddah", "Mecca", "Dammam", "Khobar" };
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> newImages)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var existing = await _context.Properties.FindAsync(id);
            if (existing == null) return NotFound();
            if (existing.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            existing.Title = property.Title;
            existing.Description = property.Description;
            existing.Price = property.Price;
            existing.City = property.City;
            existing.Bedrooms = property.Bedrooms;
            existing.Bathrooms = property.Bathrooms;
            existing.Area = property.Area;
            existing.Status = PropertyStatus.Pending;

            if (newImages != null && newImages.Count > 0)
            {
                foreach (var image in newImages)
                {
                    var imageUrl = await UploadImage(image);
                    if (imageUrl != null)
                    {
                        _context.PropertyImages.Add(new PropertyImage
                        {
                            PropertyId = id,
                            ImageUrl = imageUrl
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProperties));
        }

        // عقاراتي
        public async Task<IActionResult> MyProperties()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var properties = await _context.Properties
                .Where(p => p.UserId == userId.Value)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(properties);
        }

        // حذف عقار
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null) return NotFound();
            if (property.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();
            if (property.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProperties));
        }

        // Search
        public async Task<IActionResult> Search(string keyword)
        {
            var result = await _context.Properties
                .Where(p => p.Status == PropertyStatus.Approved &&
                    (p.Title.Contains(keyword) || p.Description.Contains(keyword) || p.City.Contains(keyword)))
                .Include(p => p.Images)
                .ToListAsync();

            return View("Index", result);
        }

        // Filter
        public async Task<IActionResult> Filter(string city, decimal? minPrice, decimal? maxPrice, int? bedrooms)
        {
            return RedirectToAction(nameof(Index), new { city, minPrice, maxPrice, bedrooms });
        }

        // مساعد رفع الصور
        private async Task<string?> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            string folder = Path.Combine(_environment.WebRootPath, "images", "properties");
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/properties/" + fileName;
        }
    }
}