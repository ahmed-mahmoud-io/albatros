using Albatros.Data;
using Albatros.Models;
using Albatros.Models.ViewModels;
using Albatros.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Albatros.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;

        public AccountController(AppDbContext context, IEmailService emailService, IWebHostEnvironment environment)
        {
            _context = context;
            _emailService = emailService;
            _environment = environment;
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
            {
                ViewBag.Error = "يرجى ملء جميع الحقول المطلوبة بشكل صحيح";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError("FullName", "الاسم الكامل مطلوب");
                ViewBag.Error = "الاسم الكامل مطلوب";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "رقم الهاتف مطلوب");
                ViewBag.Error = "رقم الهاتف مطلوب";
                return View(model);
            }

            if (model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "كلمة المرور يجب أن تكون 6 أحرف على الأقل");
                ViewBag.Error = "كلمة المرور يجب أن تكون 6 أحرف على الأقل";
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "كلمة المرور غير متطابقة");
                ViewBag.Error = "كلمة المرور غير متطابقة";
                return View(model);
            }

            bool exists = await _context.ApplicationUsers
                .AnyAsync(x => x.Email == model.Email);

            if (exists)
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني مسجل مسبقاً");
                ViewBag.Error = "البريد الإلكتروني مسجل مسبقاً";
                return View(model);
            }

            var token = Random.Shared.Next(100000, 999999).ToString();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Store registration data temporarily — NOT saved to DB until OTP verified
            var regData = new Dictionary<string, string>
            {
                ["FullName"] = model.FullName,
                ["Email"] = model.Email,
                ["PasswordHash"] = passwordHash,
                ["PhoneNumber"] = model.PhoneNumber,
                ["Role"] = model.Role == "Owner" ? "Owner" : "User"
            };
            TempData["RegistrationData"] = System.Text.Json.JsonSerializer.Serialize(regData);
            TempData["Reg_Token"] = token;

            // Send registration email
            var emailSubject = "تفعيل حسابك في ALBATROS";
            var emailBody = $@"
                <div style='direction: rtl; font-family: Cairo, Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 30px; border: 1px solid #E4E1DC; border-radius: 12px; background-color: #FFFFFF; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #9C2A2A; margin: 0;'>مرحباً بك في ALBATROS</h2>
                        <p style='color: #8A8580; margin-top: 5px; font-size: 14px;'>بوابتك العقارية الفاخرة</p>
                    </div>
                    <hr style='border: 0; border-top: 1px solid #E4E1DC; margin: 20px 0;'>
                    <p style='font-size: 15px; color: #0A0A0A; line-height: 1.6;'>عزيزنا المشترك، شكراً لتسجيلك في منصتنا. يرجى استخدام رمز التحقق (OTP) التالي لتفعيل حسابك:</p>
                    <div style='background: #F8F8F8; padding: 15px; border-radius: 10px; text-align: center; margin: 25px 0; border: 1px solid #E4E1DC;'>
                        <span style='font-size: 36px; font-weight: bold; letter-spacing: 6px; color: #9C2A2A; font-family: monospace;'>{token}</span>
                    </div>
                    <p style='font-size: 13px; color: #8A8580; text-align: center; margin-top: 20px;'>إذا لم تقم بإنشاء هذا الحساب، يرجى تجاهل هذا البريد الإلكتروني.</p>
                </div>";
            await _emailService.SendEmailAsync(model.Email, emailSubject, emailBody);

            return RedirectToAction(nameof(VerificationSent), new { email = model.Email });
        }

        // Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");

            // Look for any success messages in TempData
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "يرجى إدخال البريد الإلكتروني وكلمة المرور";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني مطلوب");
                ViewBag.Error = "البريد الإلكتروني مطلوب";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "كلمة المرور مطلوبة");
                ViewBag.Error = "كلمة المرور مطلوبة";
                return View(model);
            }

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني غير مسجل");
                ViewBag.Error = "البريد الإلكتروني غير مسجل";
                return View(model);
            }

            bool isPasswordCorrect = false;
            if (!string.IsNullOrEmpty(user.Password))
            {
                try
                {
                    isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    isPasswordCorrect = false;
                }
            }

            if (!isPasswordCorrect)
            {
                ModelState.AddModelError("Password", "كلمة المرور غير صحيحة");
                ViewBag.Error = "كلمة المرور غير صحيحة";
                return View(model);
            }

            if (!user.IsEmailVerified)
            {
                ViewBag.Error = "لم يتم تفعيل حسابك بعد. يرجى تفعيل البريد الإلكتروني.";
                ViewBag.ResendEmail = user.Email;
                return View(model);
            }

            await SignInUser(user, model.RememberMe);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        // Verification Sent Screen GET
        public IActionResult VerificationSent(string email)
        {
            var json = TempData.Peek("RegistrationData")?.ToString();
            if (string.IsNullOrEmpty(json))
                return RedirectToAction(nameof(Register));

            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data == null || !data.ContainsKey("Email") || data["Email"] != email)
                return RedirectToAction(nameof(Register));

            ViewBag.Email = email;
            ViewBag.Token = TempData.Peek("Reg_Token")?.ToString();
            TempData.Keep("RegistrationData");
            TempData.Keep("Reg_Token");
            return View();
        }

        // Verify OTP POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(string email, string code)
        {
            var json = TempData["RegistrationData"]?.ToString();
            var storedToken = TempData["Reg_Token"]?.ToString();

            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(storedToken))
            {
                ViewBag.Error = "انتهت صلاحية جلسة التسجيل. يرجى التسجيل مرة أخرى.";
                return RedirectToAction(nameof(Register));
            }

            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data == null || !data.ContainsKey("Email") || data["Email"] != email)
                return RedirectToAction(nameof(Register));

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                ViewBag.Error = "الرجاء إدخال رمز التحقق";
                ViewBag.Email = email;
                TempData.Keep("RegistrationData");
                TempData.Keep("Reg_Token");
                return View("VerificationSent");
            }

            if (storedToken != code.Trim())
            {
                ViewBag.Error = "رمز التحقق غير صحيح";
                ViewBag.Email = email;
                TempData.Keep("RegistrationData");
                TempData.Keep("Reg_Token");
                return View("VerificationSent");
            }

            // OTP correct — save user to DB
            var user = new ApplicationUser
            {
                FullName = data["FullName"],
                Email = data["Email"],
                Password = data["PasswordHash"],
                PhoneNumber = data["PhoneNumber"],
                Role = data["Role"],
                IsEmailVerified = true,
                EmailVerificationToken = null
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();

            TempData.Remove("RegistrationData");
            TempData.Remove("Reg_Token");

            TempData["SuccessMessage"] = "تم تفعيل حسابك بنجاح! يمكنك الآن تسجيل الدخول.";
            return RedirectToAction(nameof(Login));
        }

        // Verify Email Action (Direct Link for local testing)
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            var storedToken = TempData.Peek("Reg_Token")?.ToString();
            var json = TempData.Peek("RegistrationData")?.ToString();

            if (!string.IsNullOrEmpty(storedToken) && storedToken == token && !string.IsNullOrEmpty(json))
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (data != null && data.ContainsKey("Email"))
                {
                    var user = new ApplicationUser
                    {
                        FullName = data["FullName"],
                        Email = data["Email"],
                        Password = data["PasswordHash"],
                        PhoneNumber = data["PhoneNumber"],
                        Role = data["Role"],
                        IsEmailVerified = true,
                        EmailVerificationToken = null
                    };

                    _context.ApplicationUsers.Add(user);
                    await _context.SaveChangesAsync();

                    TempData.Remove("RegistrationData");
                    TempData.Remove("Reg_Token");
                    return View();
                }
            }

            // Fallback: try DB lookup for already-created users
            var dbUser = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.EmailVerificationToken == token);

            if (dbUser == null)
            {
                ViewBag.Error = "رمز تفعيل البريد الإلكتروني غير صالح أو منتهي الصلاحية";
                return View();
            }

            dbUser.IsEmailVerified = true;
            dbUser.EmailVerificationToken = null;
            await _context.SaveChangesAsync();

            return View();
        }

        // Resend Verification
        public async Task<IActionResult> ResendVerification(string email)
        {
            var json = TempData.Peek("RegistrationData")?.ToString();
            if (string.IsNullOrEmpty(json))
                return RedirectToAction(nameof(Register));

            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data == null || !data.ContainsKey("Email") || data["Email"] != email)
                return RedirectToAction(nameof(Register));

            var token = Random.Shared.Next(100000, 999999).ToString();
            TempData["Reg_Token"] = token;
            TempData.Keep("RegistrationData");

            // Send registration email again
            var emailSubject = "إعادة إرسال كود التفعيل - ALBATROS";
            var emailBody = $@"
                <div style='direction: rtl; font-family: Cairo, Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 30px; border: 1px solid #E4E1DC; border-radius: 12px; background-color: #FFFFFF; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #9C2A2A; margin: 0;'>رمز تفعيل جديد - ALBATROS</h2>
                    </div>
                    <hr style='border: 0; border-top: 1px solid #E4E1DC; margin: 20px 0;'>
                    <p style='font-size: 15px; color: #0A0A0A;'>يرجى استخدام رمز التحقق (OTP) التالي لتفعيل حسابك:</p>
                    <div style='background: #F8F8F8; padding: 15px; border-radius: 10px; text-align: center; margin: 25px 0; border: 1px solid #E4E1DC;'>
                        <span style='font-size: 36px; font-weight: bold; letter-spacing: 6px; color: #9C2A2A; font-family: monospace;'>{token}</span>
                    </div>
                </div>";
            await _emailService.SendEmailAsync(email, emailSubject, emailBody);

            return RedirectToAction(nameof(VerificationSent), new { email = email });
        }

        // Forgot Password GET
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot Password POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            string? token = null;
            if (user != null)
            {
                token = Random.Shared.Next(100000, 999999).ToString();
                user.PasswordResetToken = token;
                user.ResetTokenExpiration = DateTime.Now.AddMinutes(15);
                await _context.SaveChangesAsync();

                // Send reset OTP email
                var emailSubject = "إعادة تعيين كلمة المرور - ALBATROS";
                var emailBody = $@"
                    <div style='direction: rtl; font-family: Cairo, Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 30px; border: 1px solid #E4E1DC; border-radius: 12px; background-color: #FFFFFF; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
                        <div style='text-align: center; margin-bottom: 20px;'>
                            <h2 style='color: #9C2A2A; margin: 0;'>إعادة تعيين كلمة المرور</h2>
                        </div>
                        <hr style='border: 0; border-top: 1px solid #E4E1DC; margin: 20px 0;'>
                        <p style='font-size: 15px; color: #0A0A0A; line-height: 1.6;'>تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك. يرجى استخدام رمز التحقق (OTP) التالي لإكمال العملية:</p>
                        <div style='background: #F8F8F8; padding: 15px; border-radius: 10px; text-align: center; margin: 25px 0; border: 1px solid #E4E1DC;'>
                            <span style='font-size: 36px; font-weight: bold; letter-spacing: 6px; color: #9C2A2A; font-family: monospace;'>{token}</span>
                        </div>
                        <p style='color: #8A8580; font-size: 13px; text-align: center;'>هذا الرمز صالح لمدة 15 دقيقة فقط.</p>
                    </div>";
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation), new { email = model.Email, token = token });
        }

        // Forgot Password Confirmation Screen
        public IActionResult ForgotPasswordConfirmation(string email, string? token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        // Reset Password GET
        public IActionResult ResetPassword(string? email, string? token)
        {
            var model = new ResetPasswordViewModel 
            { 
                Email = email ?? "", 
                Token = token ?? "" 
            };
            return View(model);
        }

        // Reset Password POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.PasswordResetToken == model.Token.Trim() && x.ResetTokenExpiration > DateTime.Now);

            if (user == null)
            {
                ViewBag.Error = "رمز إعادة تعيين كلمة المرور غير صالح، منتهي الصلاحية أو البريد الإلكتروني غير صحيح";
                return View(model);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiration = null;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        // Reset Password Confirmation Screen
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
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
        public async Task<IActionResult> EditProfile(int userId, string fullName, string phoneNumber, IFormFile? profilePicture)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null || id != userId)
                return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var folder = Path.Combine(_environment.WebRootPath, "images", "avatars");
                Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid() + Path.GetExtension(profilePicture.FileName);
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }
                user.ProfilePictureUrl = "/images/avatars/" + fileName;
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("ProfilePictureUrl", user.ProfilePictureUrl ?? "");

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return NotFound();

            // Delete old file if it was uploaded (not external URL)
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && user.ProfilePictureUrl.StartsWith("/images/avatars/"))
            {
                var filePath = Path.Combine(_environment.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            user.ProfilePictureUrl = null;
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("ProfilePictureUrl", "");

            return RedirectToAction(nameof(EditProfile));
        }

        // Google Login
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback));
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync("ExternalCookie");

            if (!result.Succeeded)
                return RedirectToAction(nameof(Login));

            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var picture = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(Login));

            // Find existing user by GoogleId or Email
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.GoogleId == googleId || u.Email == email);

            bool isNew = false;
            if (user == null)
            {
                // Create new user with Google info
                user = new ApplicationUser
                {
                    FullName = name ?? email.Split('@')[0],
                    Email = email,
                    Password = null,
                    PhoneNumber = null,
                    Role = "User",
                    IsEmailVerified = true,
                    GoogleId = googleId,
                    ProfilePictureUrl = picture
                };
                _context.ApplicationUsers.Add(user);
                await _context.SaveChangesAsync();
                isNew = true;
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                // Link Google account to existing user
                user.GoogleId = googleId;
                user.ProfilePictureUrl = picture;
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrEmpty(picture) && string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = picture;
                await _context.SaveChangesAsync();
            }

            await SignInUser(user, rememberMe: true);

            if (isNew)
                return RedirectToAction(nameof(GoogleCompleteRegistration));

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Home");
        }

        // Google signup completion (collect remaining info)
        public async Task<IActionResult> GoogleCompleteRegistration()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return RedirectToAction(nameof(Login));

            // If already has phone + role set, skip
            if (!string.IsNullOrEmpty(user.PhoneNumber) && user.Role != "User")
                return RedirectToAction("Index", "Home");

            ViewBag.FullName = user.FullName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoogleCompleteRegistration(string fullName, string phoneNumber, string role)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction(nameof(Login));

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return RedirectToAction(nameof(Login));

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.Role = role == "Owner" ? "Owner" : "User";

            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
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
            HttpContext.Session.SetString("ProfilePictureUrl", user.ProfilePictureUrl ?? "");
        }
    }
}