using Albatros.Data;
using Albatros.Models;
using Albatros.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Albatros.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool exists = await _context.ApplicationUsers
                .AnyAsync(x => x.Email == model.Email);

            if (exists)
            {
                ViewBag.Error = "البريد الإلكتروني مسجل مسبقاً";
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                Role = model.Role == "Owner" ? "Owner" : "User"
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();

            // Auto-login after registration
            await SignInUser(user);

            return RedirectToAction("Index", "Home");
        }

        // Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ViewBag.Error = "البريد الإلكتروني أو كلمة المرور غير صحيحة";
                return View(model);
            }

            await SignInUser(user, model.RememberMe);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // Profile
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

        // Edit Profile
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
        public async Task<IActionResult> EditProfile(int userId, string fullName, string phoneNumber)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null || id != userId)
                return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("FullName", user.FullName);

            return RedirectToAction(nameof(Profile));
        }

        // Helper: sign in with cookie + session
        private async Task SignInUser(ApplicationUser user, bool rememberMe = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = rememberMe });

            // Also set session for backward compatibility
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);
        }
    }
}