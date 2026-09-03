using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SereGlowy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        public IActionResult EditProduct(int id)
        {
            return View();
        }

        public IActionResult Categories()
        {
            return View();
        }
        public IActionResult Ingredients()
        {
            return View();
        }
        public IActionResult MakeupProducts()
        {
            return View();
        }
    }
}