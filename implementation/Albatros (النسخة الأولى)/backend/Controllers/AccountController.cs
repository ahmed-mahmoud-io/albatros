using Albatros.Data;
using Albatros.Models;
using Albatros.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ============
        // Register
        // ============

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUser user, string userType)
        {
            // Password is bound separately below (after hashing) so remove it from
            // model validation on the incoming plain-text value.
            ModelState.Remove(nameof(ApplicationUser.Role));
            ModelState.Remove(nameof(ApplicationUser.UserType));

            if (!ModelState.IsValid)
                return View(user);

            bool exists = await _context.ApplicationUsers
                .AnyAsync(x => x.Email == user.Email);

            if (exists)
            {
                ViewBag.Error = "Email already exists";
                return View(user);
            }

            user.Role = "User";
            user.UserType = (userType == "seller") ? "Seller" : "Buyer";
            user.Password = PasswordHasher.Hash(user.Password);

            _context.ApplicationUsers.Add(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }

        // ============
        // Login
        // ============

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null || !PasswordHasher.Verify(password, user.Password))
            {
                ViewBag.Error = "Invalid Email or Password";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("UserType", user.UserType);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        // ============
        // Logout
        // ============

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Login));
        }

        // ============
        // Profile
        // ============

        public async Task<IActionResult> Profile()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // ============
        // Edit Profile
        // ============

        public async Task<IActionResult> EditProfile()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers.FindAsync(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ApplicationUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Don't let a re-posted (already-hashed) password get re-hashed on every save,
            // but DO hash it if the person actually entered a new one.
            var existing = await _context.ApplicationUsers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == user.UserId);

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = existing?.Password;
            }
            else
            {
                user.Password = PasswordHasher.Hash(user.Password);
            }

            _context.ApplicationUsers.Update(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }
    }
}
