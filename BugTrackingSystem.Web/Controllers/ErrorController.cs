using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public new IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("Error/500")]
        public IActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}