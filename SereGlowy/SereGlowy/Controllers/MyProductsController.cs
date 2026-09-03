using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    public class MyProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}