using Albatros.Data;
using Albatros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class PropertyImagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PropertyImagesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int propertyId, IFormFile image)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return NotFound();
            if (property.UserId != userId) return Forbid();

            if (image != null && image.Length > 0)
            {
                var folder = Path.Combine(_environment.WebRootPath, "images", "properties");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                _context.PropertyImages.Add(new PropertyImage
                {
                    PropertyId = propertyId,
                    ImageUrl = "/images/properties/" + fileName
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", "Properties", new { id = propertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var image = await _context.PropertyImages
                .Include(pi => pi.Property)
                .FirstOrDefaultAsync(pi => pi.ImageId == id);

            if (image == null) return NotFound();
            if (image.Property.UserId != userId) return Forbid();

            var filePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            int propertyId = image.PropertyId;
            _context.PropertyImages.Remove(image);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Properties", new { id = propertyId });
        }
    }
}