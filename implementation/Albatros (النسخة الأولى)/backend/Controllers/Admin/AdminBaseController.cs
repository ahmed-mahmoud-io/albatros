using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Albatros.Controllers.Admin
{
    // All Admin/* controllers inherit from this instead of using [Authorize(Roles="Admin")],
    // because this project doesn't use ASP.NET Identity/cookie auth -- it uses a simple
    // session flag ("Role") set in AccountController.Login. [Authorize] alone would throw
    // at runtime here since no authentication scheme is registered.
    //
    // [Area("Admin")] is REQUIRED here, not cosmetic: PropertiesController, ReviewsController
    // and VisitRequestsController all exist twice (once public, once under Controllers/Admin)
    // with the identical class name. Without an Area, ASP.NET Core's default routing can't
    // tell the two "PropertiesController"s apart and throws an AmbiguousMatchException the
    // first time either route is hit. This attribute is inherited by every controller below.
    [Area("Admin")]
    public abstract class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
