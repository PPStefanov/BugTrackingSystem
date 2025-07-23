using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    ViewBag.StatusCode = statusCode;
                    return View("NotFound");
                case 500:
                    ViewBag.ErrorMessage = "Sorry, something went wrong on the server";
                    ViewBag.StatusCode = statusCode;
                    return View("ServerError");
                default:
                    ViewBag.ErrorMessage = "An error occurred while processing your request";
                    ViewBag.StatusCode = statusCode;
                    return View("Error");
            }
        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}