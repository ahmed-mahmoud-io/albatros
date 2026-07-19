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

    // Seed 21 properties if none exist
    var seller = context.ApplicationUsers.First(u => u.Email == "seller@albatros.sa");
    if (!context.Properties.Any())
    {
        var imageSets = new Dictionary<int, string[]>
        {
            [1] = new[] { "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800" },
            [2] = new[] { "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800" },
            [3] = new[] { "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800" },
            [4] = new[] { "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800" },
            [5] = new[] { "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800" },
            [6] = new[] { "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800" },
            [7] = new[] { "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800" },
            [8] = new[] { "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800" },
            [9] = new[] { "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800" },
            [10] = new[] { "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800" },
            [11] = new[] { "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800" },
            [12] = new[] { "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800" },
            [13] = new[] { "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800" },
            [14] = new[] { "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800" },
            [15] = new[] { "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800" },
            [16] = new[] { "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800" },
            [17] = new[] { "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800" },
            [18] = new[] { "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800" },
            [19] = new[] { "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800", "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800" },
            [20] = new[] { "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800", "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800" },
            [21] = new[] { "https://images.unsplash.com/photo-1600573472550-8090b5e0745e?w=800", "https://images.unsplash.com/photo-1600210491892-03d54c0aaf87?w=800", "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800" },
        };

        var properties = new (string Title, string Desc, decimal Price, string City, int Bed, int Bath, double Area, int ListingType, int Status)[]
        {
            ("Villa in Riyadh 1", "A beautiful Villa located in Riyadh with modern amenities.", 650000.00m, "Riyadh", 2, 1, 175.0, 0, 1),
            ("Apartment in Jeddah 2", "A beautiful Apartment located in Jeddah with modern amenities.", 8000.00m, "Jeddah", 3, 2, 200.0, 1, 1),
            ("Duplex in Dammam 3", "A beautiful Duplex located in Dammam with modern amenities.", 950000.00m, "Dammam", 4, 3, 225.0, 0, 1),
            ("Palace in Mecca 4", "A beautiful Palace located in Mecca with modern amenities.", 15000.00m, "Mecca", 5, 1, 250.0, 1, 1),
            ("Chalet in Khobar 5", "A beautiful Chalet located in Khobar with modern amenities.", 1250000.00m, "Khobar", 2, 2, 275.0, 0, 1),
            ("Villa in Riyadh 6", "A beautiful Villa located in Riyadh with modern amenities.", 12000.00m, "Riyadh", 3, 3, 300.0, 1, 1),
            ("Apartment in Jeddah 7", "A beautiful Apartment located in Jeddah with modern amenities.", 1550000.00m, "Jeddah", 4, 1, 325.0, 0, 1),
            ("Duplex in Dammam 8", "A beautiful Duplex located in Dammam with modern amenities.", 9000.00m, "Dammam", 5, 2, 350.0, 1, 1),
            ("Palace in Mecca 9", "A beautiful Palace located in Mecca with modern amenities.", 1850000.00m, "Mecca", 2, 3, 375.0, 0, 1),
            ("Chalet in Khobar 10", "A beautiful Chalet located in Khobar with modern amenities.", 10000.00m, "Khobar", 3, 1, 400.0, 1, 1),
            ("Villa in Riyadh 11", "A beautiful Villa located in Riyadh with modern amenities.", 2150000.00m, "Riyadh", 4, 2, 425.0, 0, 1),
            ("Apartment in Jeddah 12", "A beautiful Apartment located in Jeddah with modern amenities.", 8500.00m, "Jeddah", 5, 3, 450.0, 1, 1),
            ("Duplex in Dammam 13", "A beautiful Duplex located in Dammam with modern amenities.", 2450000.00m, "Dammam", 2, 1, 475.0, 0, 1),
            ("Palace in Mecca 14", "A beautiful Palace located in Mecca with modern amenities.", 18000.00m, "Mecca", 3, 2, 500.0, 1, 1),
            ("Chalet in Khobar 15", "A beautiful Chalet located in Khobar with modern amenities.", 2750000.00m, "Khobar", 4, 3, 525.0, 0, 1),
            ("Villa in Riyadh 16", "A beautiful Villa located in Riyadh with modern amenities.", 14000.00m, "Riyadh", 5, 1, 550.0, 1, 1),
            ("Apartment in Jeddah 17", "A beautiful Apartment located in Jeddah with modern amenities.", 3050000.00m, "Jeddah", 2, 2, 575.0, 0, 1),
            ("Duplex in Dammam 18", "A beautiful Duplex located in Dammam with modern amenities.", 9500.00m, "Dammam", 3, 3, 600.0, 1, 1),
            ("Palace in Mecca 19", "A beautiful Palace located in Mecca with modern amenities.", 3350000.00m, "Mecca", 4, 1, 625.0, 0, 1),
            ("Chalet in Khobar 20", "A beautiful Chalet located in Khobar with modern amenities.", 11000.00m, "Khobar", 5, 2, 650.0, 1, 1),
            ("Villa in Riyadh 21", "A beautiful Villa located in Riyadh with modern amenities.", 4500000.00m, "Riyadh", 6, 5, 550.0, 0, 1),
        };

        int pid = 0;
        foreach (var p in properties)
        {
            pid++;
            var prop = new Property
            {
                Title = p.Title,
                Description = p.Desc,
                Price = p.Price,
                City = p.City,
                Bedrooms = p.Bed,
                Bathrooms = p.Bath,
                Area = p.Area,
                ListingType = (ListingType)p.ListingType,
                Status = (PropertyStatus)p.Status,
                UserId = seller.UserId,
                CreatedAt = new DateTime(2026, 7, 19, 22, 52, 08)
            };
            context.Properties.Add(prop);
            context.SaveChanges();

            if (imageSets.TryGetValue(pid, out var urls))
            {
                foreach (var url in urls)
                {
                    context.PropertyImages.Add(new PropertyImage { PropertyId = prop.PropertyId, ImageUrl = url });
                }
            }
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
