using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
