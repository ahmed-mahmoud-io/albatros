using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    // Was [Authorize] at the class level -- this project has no authentication scheme
    // registered (no AddAuthentication/AddIdentity in Program.cs), so that attribute alone
    // throws at runtime. Replaced with the same manual session check used everywhere else.
    public class PropertyImagesController : Controller
    {
        private readonly AppDbContext _context;

        public PropertyImagesController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        // View images for a property -- public
        public async Task<IActionResult> Index(int propertyId)
        {
            var images = await _context.PropertyImages
                .Where(i => i.PropertyId == propertyId)
                .ToListAsync();

            ViewBag.PropertyId = propertyId;

            return View(images);
        }

        // Add an image -- requires login
        public IActionResult Create(int propertyId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            PropertyImage image = new PropertyImage
            {
                PropertyId = propertyId
            };

            return View(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyImage image)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.PropertyImages.Add(image);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { propertyId = image.PropertyId });
            }

            return View(image);
        }

        // Edit an image -- requires login
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var image = await _context.PropertyImages.FindAsync(id);

            if (image == null)
                return NotFound();

            return View(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyImage image)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id != image.ImageId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { propertyId = image.PropertyId });
            }

            return View(image);
        }

        // Delete an image -- requires login
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var image = await _context.PropertyImages
                .FirstOrDefaultAsync(i => i.ImageId == id);

            if (image == null)
                return NotFound();

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var image = await _context.PropertyImages.FindAsync(id);

            if (image == null)
                return NotFound();

            int propertyId = image.PropertyId;

            _context.PropertyImages.Remove(image);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { propertyId });
        }
    }
}
