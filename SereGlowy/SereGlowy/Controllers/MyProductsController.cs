using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;

namespace SereGlowy.Controllers
{
    [Authorize]
    public class MyProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyProductsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // My Products - READ
        // =========================
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var myProducts = await _context.MyProducts
                .Where(mp => mp.UserId == userId)
                .Include(mp => mp.Product)
                    .ThenInclude(p => p.Category)
                .OrderByDescending(mp => mp.AddedAt)
                .ToListAsync();

            ViewBag.SavedCount = myProducts.Count;

            ViewBag.CategoryCount = myProducts
                .Where(mp => mp.Product?.Category != null)
                .Select(mp => mp.Product!.CategoryId)
                .Distinct()
                .Count();

            return View(myProducts);
        }


        // =========================
        // Add Product
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == productId);

            if (!productExists)
            {
                return NotFound();
            }

            var alreadySaved = await _context.MyProducts
                .AnyAsync(mp =>
                    mp.UserId == userId &&
                    mp.ProductId == productId);

            if (!alreadySaved)
            {
                var myProduct = new MyProduct
                {
                    UserId = userId,
                    ProductId = productId,
                    AddedAt = DateTime.Now
                };

                _context.MyProducts.Add(myProduct);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Product added to My Products.";
            }
            else
            {
                TempData["InfoMessage"] =
                    "This product is already in My Products.";
            }

            return RedirectToAction(
                "Index",
                "Products");
        }


        // =========================
        // Remove Product
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var myProduct = await _context.MyProducts
                .FirstOrDefaultAsync(mp =>
                    mp.MyProductId == id &&
                    mp.UserId == userId);

            if (myProduct == null)
            {
                return NotFound();
            }

            _context.MyProducts.Remove(myProduct);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Product removed from My Products.";

            return RedirectToAction(nameof(Index));
        }
    }
}