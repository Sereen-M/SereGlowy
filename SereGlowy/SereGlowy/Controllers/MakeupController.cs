using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class MakeupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}