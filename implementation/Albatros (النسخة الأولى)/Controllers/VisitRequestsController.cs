using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class VisitRequestsController : Controller
    {
        private readonly AppDbContext _context;

        public VisitRequestsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var requests = await _context.VisitRequests
                .Where(v => v.UserId == userId.Value)
                .Include(v => v.Property)
                    .ThenInclude(p => p.Images)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return View(requests);
        }

        public IActionResult Create(int propertyId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var request = new VisitRequest
            {
                PropertyId = propertyId,
                VisitDate = DateTime.Now.AddDays(1)
            };
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            request.UserId = userId.Value;
            request.Status = VisitRequestStatus.Pending;

            _context.VisitRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Properties", new { id = request.PropertyId });
        }

        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var request = await _context.VisitRequests
                .Include(v => v.Property)
                    .ThenInclude(p => p.Images)
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null) return NotFound();
            if (request.UserId != userId && HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            return View(request);
        }

        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var request = await _context.VisitRequests
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null) return NotFound();
            if (request.UserId != userId) return Forbid();

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var request = await _context.VisitRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (request.UserId != userId) return Forbid();

            _context.VisitRequests.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}