using Albatros.Data;
using Albatros.Models;
using Albatros.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailService, EmailService>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// External cookie scheme for OAuth providers
builder.Services.AddAuthentication()
    .AddCookie("ExternalCookie", options =>
    {
        options.Cookie.Name = "ExternalAuth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

// Cookie Authentication + Google OAuth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    })
    .AddGoogle(options =>
    {
        options.SignInScheme = "ExternalCookie";
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed 3 default accounts + run migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    var seedUsers = new List<(string Email, string FullName, string Password, string Role)>
    {
        ("admin@albatros.sa", "System Administrator", "Admin@123", "Admin"),
        ("seller@albatros.sa",  "Property Seller",     "Seller@123", "Owner"),
        ("buyer@albatros.sa",   "Client Buyer",        "Buyer@123",  "User"),
    };

    foreach (var (email, fullName, password, role) in seedUsers)
    {
        var existing = context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
        if (existing == null)
        {
            context.ApplicationUsers.Add(new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                PhoneNumber = "0500000000",
                Role = role,
                IsEmailVerified = true
            });
        }
        else
        {
            existing.FullName = fullName;
            existing.Email = email;
            existing.Password = BCrypt.Net.BCrypt.HashPassword(password);
            existing.Role = role;
            existing.IsEmailVerified = true;
        }
    }
    context.SaveChanges();

    // Remove property 24 (البالب)
    var prop24 = context.Properties.Find(24);
    if (prop24 != null)
    {
        context.PropertyImages.RemoveRange(context.PropertyImages.Where(i => i.PropertyId == 24));
        context.Favorites.RemoveRange(context.Favorites.Where(f => f.PropertyId == 24));
        context.Reviews.RemoveRange(context.Reviews.Where(r => r.PropertyId == 24));
        context.VisitRequests.RemoveRange(context.VisitRequests.Where(v => v.PropertyId == 24));
        context.Properties.Remove(prop24);
        context.SaveChanges();
    }

    // Reassign all properties to the seller
    var seller = context.ApplicationUsers.First(u => u.Email == "seller@albatros.sa");
    var orphanProps = context.Properties.Where(p => p.UserId != seller.UserId).ToList();
    foreach (var p in orphanProps) p.UserId = seller.UserId;
    context.SaveChanges();

    // Seed images for "Duplex in Dammam 18" - replace old loremflickr images with Unsplash
    var duplexProp = context.Properties
        .Include(p => p.Images)
        .FirstOrDefault(p => p.Title.Contains("Duplex in Dammam 18"));
    if (duplexProp != null)
    {
        // Remove any old loremflickr images first
        var oldDuplexImages = duplexProp.Images?.Where(i => i.ImageUrl.Contains("loremflickr")).ToList();
        if (oldDuplexImages != null && oldDuplexImages.Count > 0)
        {
            context.PropertyImages.RemoveRange(oldDuplexImages);
            context.SaveChanges();
        }

        // Re-fetch to get updated state
        context.Entry(duplexProp).Collection(p => p.Images).Load();
    }
    if (duplexProp != null && (duplexProp.Images == null || duplexProp.Images.Count == 0))
    {
        var duplexImages = new[]
        {
            "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800",
            "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800",
            "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800",
            "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800",
            "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800"
        };
        foreach (var url in duplexImages)
        {
            context.PropertyImages.Add(new PropertyImage
            {
                PropertyId = duplexProp.PropertyId,
                ImageUrl = url
            });
        }
        context.SaveChanges();
    }

    // One-time cleanup: when SeedMode = Reset, remove all non-seed users
    if (builder.Configuration["SeedMode"] == "Reset")
    {
        var seedEmails = new[] { "admin@albatros.sa", "seller@albatros.sa", "buyer@albatros.sa" };
        var extraIds = context.ApplicationUsers
            .Where(u => !seedEmails.Contains(u.Email))
            .Select(u => (int?)u.UserId)
            .ToList();
        var ids = string.Join(",", extraIds.Where(id => id.HasValue).Select(id => id.Value));
        if (!string.IsNullOrEmpty(ids))
        {
            context.Database.ExecuteSqlRaw($@"
                DELETE FROM VisitRequests WHERE UserId IN ({ids});
                DELETE FROM Favorites WHERE UserId IN ({ids});
                DELETE FROM Reviews WHERE UserId IN ({ids});
                DELETE FROM Notifications WHERE UserId IN ({ids});
                DELETE FROM ApplicationUsers WHERE UserId IN ({ids});
            ");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Areas route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
