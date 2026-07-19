# ALBATROS backend — what I fixed and what's left

I unzipped your project, read through every controller/model, and it wouldn't have
compiled or run as uploaded. Below is everything I found and fixed, and a short list
of what's still worth doing.

## Critical bugs (app wouldn't start / would crash) — fixed

1. **appsettings.json was invalid JSON** — `ConnectionStrings` was a second root
   object glued on after `AllowedHosts`. The app would fail before it even started.
2. **`AppDbContext` was never registered in `Program.cs`** — every controller injects
   it, but there was no `AddDbContext<AppDbContext>(...)` call. Fixed.
3. **Session was never registered** — `AccountController`, `VisitRequestsController`,
   etc. all read `HttpContext.Session`, but `AddSession()`/`UseSession()` were missing.
   Fixed.
4. **View files didn't match their action names.** ASP.NET Core resolves views by
   convention (`Index()` → `Index.cshtml`). Three views were misnamed and would have
   thrown "view not found":
   - `Views/Home/Home.cshtml` → renamed to `Index.cshtml`
   - `Views/Properties/Properties.cshtml` → renamed to `Index.cshtml`
   - `Views/Properties/propertiesdetails.cshtml` → renamed to `Details.cshtml`
5. **`@model` type mismatches** — `Home/Index` and `Properties/Index` both declared
   `@model Property` (singular) but their controllers pass a `List<Property>`. Fixed
   to `IEnumerable<Property>`, and the hardcoded 3/9 property cards were replaced
   with real `@foreach` loops over the database.
6. **Double `<html>` nesting** — `_ViewStart.cshtml` applies `_Layout.cshtml` to every
   view by default, but `Home`, `Properties/Index` and `Properties/Details` are full
   standalone documents. Added `@{ Layout = null; }` to each.
7. **`FavoritesController` and `Admin/UsersController` used `UserManager<ApplicationUser>`
   and `[Authorize]`**, but this project has no ASP.NET Identity configured at all, and
   `ApplicationUser` doesn't inherit `IdentityUser`. This is a compile error. Rewrote both
   to use the same simple session-based pattern as the rest of the app.
8. **`Admin/DashboardController` read `_context.Users`**, which doesn't exist on
   `AppDbContext` (it's `_context.ApplicationUsers`). Fixed.
9. **Three Admin controllers used `[Authorize(Roles = "Admin")]`** with no auth scheme
   registered → runtime crash on every request. Replaced with a shared
   `AdminBaseController` that checks `Session["Role"] == "Admin"`.
10. **Routing collision**: `PropertiesController`, `ReviewsController` and
    `VisitRequestsController` each exist twice — once public, once under
    `Controllers/Admin/` — with the *identical class name*. Without Areas, ASP.NET
    Core can't tell them apart and throws `AmbiguousMatchException` the first time
    either route is hit. Properly wired up **ASP.NET Core Areas**: added
    `[Area("Admin")]`, moved the admin views to `Areas/Admin/Views/...`, and added
    the area route in `Program.cs` (`{area:exists}/{controller}/{action}/{id?}`).
11. **`PropertyImagesController` had `[Authorize]`** with the same missing-auth-scheme
    problem as #9. Fixed the same way.
12. **Passwords were stored and compared in plain text.** Added `Security/PasswordHasher.cs`
    (PBKDF2, no extra packages needed) and wired it into Register/Login/EditProfile.

## Feature gaps — filled in

- **Buyer/Seller selection didn't exist anywhere in the schema.** Added
  `UserType` ("Buyer"/"Seller") to `ApplicationUser`, wired it into the registration
  form and `AccountController.Register`. **You'll need to add an EF Core migration**
  for this new column (`dotnet ef migrations add AddUserType`, then `dotnet ef database update`).
- Built the **missing views** so every existing controller action actually has
  something to render: `Account/Login`, `Account/Register`, `Account/Profile`,
  `Account/EditProfile`, `Favorites/Index`, `Home/Contact`, `Home/About`,
  `Home/AccessDenied`, `Properties/Create`, `Properties/Edit`, `Properties/Delete`,
  `Properties/MyProperties`, `VisitRequests/*`, `Reviews/*`, `PropertyImages/*`,
  and the full `Areas/Admin/Views/*` set (Dashboard, Properties, Users, Reviews,
  VisitRequests).
- Wired the **"Request a Viewing"** and **"Save to Favorites"** buttons on the
  property details page to the real controllers.
- Property list filters (location/price/bedrooms) now actually submit to
  `PropertiesController.Filter`.

## Known limitations / good next steps

- **Image uploads are just a URL field** (`PropertyImages/Create`). `UploadImage()`
  already exists as a private helper in `PropertiesController` but isn't wired to any
  form yet — worth connecting to a real `<input type="file">` if you want sellers to
  upload photos directly.
- **The Contact page form doesn't save anywhere** — there's no `ContactMessage` table.
  Either point it at `VisitRequestsController` for a specific property, or add a small
  model + migration for general inquiries.
- **Property approval**: new listings default to `Pending` and only show on the
  homepage once `Status` is `Approved` (the public `/Properties` list currently shows
  everything regardless of status — you may want to filter that too, for consistency).
  Admins can change status from `Areas/Admin/Views/Properties/Edit.cshtml`.
- I did **not** attempt to compile this (no .NET SDK / SQL Server available in this
  environment), so please run `dotnet build` locally before deploying — I was careful
  but a large hand-edit like this is worth a real compiler pass.
- Update the `DefaultConnection` string in `appsettings.json` for your actual SQL
  Server instance if `DESKTOP-HBPIOS7\SQLEXPRESS` isn't it.

## To run it

```
dotnet ef database update   # after adding the UserType migration mentioned above
dotnet run
```
