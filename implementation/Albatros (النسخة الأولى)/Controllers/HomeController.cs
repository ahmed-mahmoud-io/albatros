using Albatros.Data;
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

            ViewBag.TotalProperties = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Approved);
            ViewBag.TotalUsers = await _context.ApplicationUsers.CountAsync();
            ViewBag.TotalCities = await _context.Properties
                .Where(p => p.Status == PropertyStatus.Approved)
                .Select(p => p.City)
                .Distinct()
                .CountAsync();

            ViewBag.ShowSplash = true;
            return View(properties);
        }

        // Search
        [HttpGet]
        public async Task<IActionResult> Search(string city, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Properties
                .Include(p => p.Images)
                .Include(p => p.User)
                .Where(p => p.Status == PropertyStatus.Approved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(p => p.City.Contains(city));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            var properties = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View("Index", properties);
        }

        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Privacy() => View();
        public IActionResult AccessDenied() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
