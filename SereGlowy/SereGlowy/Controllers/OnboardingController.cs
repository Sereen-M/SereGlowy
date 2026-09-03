using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class OnboardingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}