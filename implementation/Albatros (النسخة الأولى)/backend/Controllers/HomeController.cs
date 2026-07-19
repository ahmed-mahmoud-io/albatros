using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Albatros.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // الصفحة الرئيسية
        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.User)
                .Where(p => p.Status == PropertyStatus.Approved)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(properties);
        }

        // search
        [HttpGet]
        public async Task<IActionResult> Search(string city, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Properties
                .Include(p => p.Images)
                .Include(p => p.User)
                .Where(p => p.Status == PropertyStatus.Approved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.Contains(city));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var properties = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View("Index", properties);
        }

        // صفحة About
        public IActionResult About()
        {
            return View();
        }

        // صفحة Contact
        public IActionResult Contact()
        {
            return View();
        }

        // صفحة Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // فى حالة عدم وجود صلاحية
        public IActionResult AccessDenied()
        {
            return View();
        }

        
    }
}
