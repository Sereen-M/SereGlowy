using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class SkinProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}