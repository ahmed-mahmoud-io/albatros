using Albatros.Data;
using Albatros.Models;
using Albatros.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Albatros.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

        [HttpGet]
        public IActionResult Contact()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string fullName, string email, string phone, string subject, string message)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction("Contact");
            }

            var msg = new ContactMessage
            {
                FullName = fullName,
                Email = email,
                Phone = phone ?? "",
                Subject = subject,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            _context.ContactMessages.Add(msg);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                "ahmed1712ax@gmail.com",
                $"New Contact Inquiry: {subject}",
                $@"
                    <h2>New Contact Message</h2>
                    <p><strong>Name:</strong> {fullName}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Phone:</strong> {phone}</p>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Message:</strong><br/>{message}</p>
                    <hr/>
                    <p style='color:#888;'>Sent via ALBATROS contact form</p>"
            );

            TempData["Success"] = "Message sent successfully! Our team will contact you shortly.";
            return RedirectToAction("Contact");
        }
        public IActionResult Privacy() => View();
        public IActionResult AccessDenied() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
