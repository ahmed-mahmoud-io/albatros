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

        //=========================
        // عرض طلبات المستخدم
        //=========================

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var requests = await _context.VisitRequests
                .Where(v => v.UserId == userId.Value)
                .Include(v => v.Property)
                .ThenInclude(p => p.Images)
                .ToListAsync();

            return View(requests);
        }

        //=========================
        // إنشاء طلب زيارة
        //=========================

        public IActionResult Create(int propertyId)
        {
            VisitRequest request = new VisitRequest
            {
                PropertyId = propertyId,
                VisitDate = DateTime.Now.AddDays(1)
            };

            return View(request);
        }

        //=========================
        // حفظ طلب الزيارة
        //=========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            request.UserId = userId.Value;
            request.Status = VisitRequestStatus.Pending;

            _context.VisitRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // تفاصيل الطلب
        //=========================

        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.VisitRequests
                .Include(v => v.Property)
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        //=========================
        // حذف الطلب
        //=========================

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.VisitRequests
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.VisitRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            _context.VisitRequests.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}