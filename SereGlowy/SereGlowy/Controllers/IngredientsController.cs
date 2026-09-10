using Microsoft.AspNetCore.Mvc;
using SereGlowy.Data;

namespace SereGlowy.Controllers
{
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ingredients = _context.Ingredients.ToList();

            return View(ingredients);
        }

        public IActionResult Details(int id)
        {
            var ingredient = _context.Ingredients
                .FirstOrDefault(i => i.IngredientId == id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }
    }
}