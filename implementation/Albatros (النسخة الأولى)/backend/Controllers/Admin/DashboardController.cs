using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Albatros.Data;

namespace Albatros.Controllers.Admin
{
    public class DashboardController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            ViewBag.UsersCount = await _context.ApplicationUsers.CountAsync();
            ViewBag.PropertiesCount = await _context.Properties.CountAsync();
            ViewBag.FavoritesCount = await _context.Favorites.CountAsync();
            ViewBag.ReviewsCount = await _context.Reviews.CountAsync();
            ViewBag.VisitRequestsCount = await _context.VisitRequests.CountAsync();
            ViewBag.PendingVisits = await _context.VisitRequests
                .CountAsync(v => v.Status == Albatros.Models.VisitRequestStatus.Pending);

            return View();
        }
    }
}
