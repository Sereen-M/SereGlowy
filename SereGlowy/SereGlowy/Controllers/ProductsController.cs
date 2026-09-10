using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;
using System.Security.Claims;

namespace SereGlowy.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // PRODUCTS
        // =========================
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }


        // =========================
        // PRODUCT DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p =>
                    p.ProductId == id &&
                    p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // =========================
        // RECOMMENDED PRODUCTS
        // =========================
        [Authorize]
        public async Task<IActionResult> Recommended()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }


            // Get the logged-in user's skin profile
            var skinProfile = await _context.SkinProfiles
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .FirstOrDefaultAsync();


            // User has not completed onboarding yet
            if (skinProfile == null)
            {
                return RedirectToAction(
                    "Index",
                    "Onboarding");
            }


            // Get products matching the user's skin type
            var recommendedProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p =>
                    p.IsActive &&
                    (
                        p.SuitableSkinType == skinProfile.SkinType ||
                        p.SuitableSkinType == "All Skin Types"
                    ))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();


            var viewModel = new RecommendedProductsViewModel
            {
                SkinProfile = skinProfile,
                Products = recommendedProducts
            };


            return View(viewModel);
        }
    }
}