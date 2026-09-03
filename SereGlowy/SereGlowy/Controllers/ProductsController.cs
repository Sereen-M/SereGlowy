using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }

        public IActionResult Recommended()
        {
            return View();
        }
    }
}