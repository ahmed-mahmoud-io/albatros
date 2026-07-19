using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    [Area("Admin")]
    public class VisitRequestsController : Controller
    {
        private readonly AppDbContext _context;

        public VisitRequestsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var requests = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var request = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
                    .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var request = await _context.VisitRequests
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.RequestId == id);
            if (request == null) return NotFound();

            request.Status = VisitRequestStatus.Approved;
            await _context.SaveChangesAsync();

            // Notify the client that their request was approved
            _context.Notifications.Add(new Notification
            {
                UserId = request.UserId,
                Message = $"✅ Your viewing request for \"{request.Property?.Title ?? "Property #" + request.PropertyId}\" on {request.VisitDate:MMM dd, yyyy - HH:mm} has been approved!",
                LinkUrl = Url.Action("Details", "Properties", new { id = request.PropertyId }),
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var request = await _context.VisitRequests
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.RequestId == id);
            if (request == null) return NotFound();

            request.Status = VisitRequestStatus.Rejected;
            await _context.SaveChangesAsync();

            // Notify the client that their request was rejected
            _context.Notifications.Add(new Notification
            {
                UserId = request.UserId,
                Message = $"❌ Your viewing request for \"{request.Property?.Title ?? "Property #" + request.PropertyId}\" on {request.VisitDate:MMM dd, yyyy - HH:mm} was not approved. Please try a different time.",
                LinkUrl = Url.Action("Details", "Properties", new { id = request.PropertyId }),
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var request = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var request = await _context.VisitRequests.FindAsync(id);
            if (request != null)
            {
                _context.VisitRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
