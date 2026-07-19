using Albatros.Data;
using Albatros.Models;
using Albatros.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class VisitRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public VisitRequestsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            var user = await _context.ApplicationUsers.FindAsync(userId.Value);
            var property = await _context.Properties.FindAsync(request.PropertyId);
            var owner = property != null ? await _context.ApplicationUsers.FindAsync(property.UserId) : null;

            // 1) Notify the property owner
            if (owner != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = owner.UserId,
                    Message = $"📅 {user?.FullName ?? "A client"} requested a viewing for your property \"{property?.Title ?? "Untitled"}\" on {request.VisitDate:MMM dd, yyyy - HH:mm}",
                    LinkUrl = Url.Action("MyProperties", "Properties"),
                    CreatedAt = DateTime.UtcNow
                });
            }

            // 2) Notify admin
            var admins = await _context.ApplicationUsers.Where(u => u.Role == "Admin").ToListAsync();
            foreach (var admin in admins)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = admin.UserId,
                    Message = $"📅 New viewing request by {user?.FullName ?? "Unknown"} for \"{property?.Title ?? "Property #" + request.PropertyId}\" on {request.VisitDate:MMM dd, yyyy - HH:mm}",
                    LinkUrl = Url.Action("Index", "VisitRequests", new { area = "Admin" }),
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            // 3) Email notification to admin
            await _emailService.SendEmailAsync(
                "ahmed1712ax@gmail.com",
                $"New Visit Request for Property #{request.PropertyId}",
                $@"
                    <h2>New Private Viewing Request</h2>
                    <p><strong>Client:</strong> {user?.FullName ?? "Unknown"}</p>
                    <p><strong>Email:</strong> {user?.Email}</p>
                    <p><strong>Phone:</strong> {user?.PhoneNumber}</p>
                    <p><strong>Property:</strong> #{request.PropertyId} - {property?.Title}</p>
                    <p><strong>Requested Date:</strong> {request.VisitDate:MMM dd, yyyy - HH:mm}</p>
                    <hr/>
                    <p style='color:#888;'>Manage this request in the admin panel.</p>"
            );

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