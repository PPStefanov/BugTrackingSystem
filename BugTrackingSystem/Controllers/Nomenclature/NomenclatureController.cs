using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Controllers.Nomenclature
{
    public class NomenclatureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}