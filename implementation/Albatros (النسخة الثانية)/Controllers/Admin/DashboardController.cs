using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.UsersCount = await _context.ApplicationUsers.CountAsync();
            ViewBag.PropertiesCount = await _context.Properties.CountAsync();
            ViewBag.FavoritesCount = await _context.Favorites.CountAsync();
            ViewBag.ReviewsCount = await _context.Reviews.CountAsync();
            ViewBag.VisitRequestsCount = await _context.VisitRequests.CountAsync();
            ViewBag.PendingProperties = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Pending);
            ViewBag.PendingVisits = await _context.VisitRequests.CountAsync(v => v.Status == VisitRequestStatus.Pending);

            ViewBag.RecentProperties = await _context.Properties
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentRequests = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
                .OrderByDescending(v => v.VisitDate)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}