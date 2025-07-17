using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Web.Controllers.Nomenclature
{
    public class NomenclatureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}