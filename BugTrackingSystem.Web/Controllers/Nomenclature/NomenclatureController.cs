using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Web.Controllers.Nomenclature
{
    [Authorize]
    public class NomenclatureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}