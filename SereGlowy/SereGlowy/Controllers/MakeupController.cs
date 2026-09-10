using Microsoft.AspNetCore.Mvc;
using SereGlowy.Data;

namespace SereGlowy.Controllers
{
    public class MakeupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MakeupController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var makeupProducts = _context.MakeupProducts
                .Where(m => m.IsActive)
                .ToList();

            return View(makeupProducts);
        }
    }
}