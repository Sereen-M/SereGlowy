using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class RoutineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}