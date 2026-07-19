
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers.Admin
{
    public class PropertiesController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public PropertiesController(AppDbContext context)
        {
            _context = context;
        }

        // عرض كل العقارات
    

        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .ToListAsync();

            return View(properties);
        }

        
        // تفاصيل العقار
   

        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound();

            return View(property);
        }

     
        // تعديل العقار
        
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties
                .FindAsync(id);

            if (property == null)
                return NotFound();

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.PropertyId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(property);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(property);
        }

        // حذف العقار
       

        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound();

            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties
                .FindAsync(id);

            if (property != null)
            {
                _context.Properties.Remove(property);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}