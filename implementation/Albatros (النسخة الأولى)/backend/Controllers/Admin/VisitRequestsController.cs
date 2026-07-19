
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    public class VisitRequestsController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public VisitRequestsController(AppDbContext context)
        {
            _context = context;
        }

        
        // عرض جميع طلبات الزيارة
   

        public async Task<IActionResult> Index()
        {
            var requests = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
                .ToListAsync();

            return View(requests);
        }

        // تفاصيل الطلب
   

        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.RequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        //=========================
        // الموافقة على الطلب
        //=========================

        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.VisitRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            request.Status = VisitRequestStatus.Approved;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
        // رفض الطلب
      

        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.VisitRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            request.Status = VisitRequestStatus.Rejected;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

      
        // حذف الطلب
       

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.VisitRequests
                .Include(v => v.User)
                .Include(v => v.Property)
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

            if (request != null)
            {
                _context.VisitRequests.Remove(request);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}