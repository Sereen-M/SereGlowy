using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class IngredientsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}