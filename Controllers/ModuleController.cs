using Microsoft.AspNetCore.Mvc;

namespace FNB_ANGULAR_API.Controllers
{
    public class ModuleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
