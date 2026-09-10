using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;

namespace SereGlowy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =====================================
        // Dashboard
        // =====================================

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.ProductCount =
                await _context.Products.CountAsync();

            ViewBag.IngredientCount =
                await _context.Ingredients.CountAsync();

            ViewBag.CategoryCount =
                await _context.Categories.CountAsync();

            ViewBag.MakeupProductCount =
                await _context.MakeupProducts.CountAsync();

            ViewBag.SkinProfileCount =
                await _context.SkinProfiles.CountAsync();

            ViewBag.UserCount =
                await _userManager.Users.CountAsync();

            return View();
        }


        // =====================================
        // PRODUCTS
        // =====================================

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }


        // Add Product - GET
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Categories =
                new SelectList(
                    await _context.Categories.ToListAsync(),
                    "CategoryId",
                    "CategoryName");

            return View();
        }

        // Add Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(
            Product product,
            IFormFile? ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "products"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(ImageFile.FileName);

                string filePath =
                    Path.Combine(
                        uploadsFolder,
                        uniqueFileName
                    );

                using (var fileStream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                product.ImageUrl =
                    "/images/products/" + uniqueFileName;
            }
            else
            {
                product.ImageUrl = "";
            }

            product.Description ??= "";
            product.SuitableSkinType ??= "";
            product.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Product added successfully.";

                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories =
                new SelectList(
                    await _context.Categories.ToListAsync(),
                    "CategoryId",
                    "CategoryName",
                    product.CategoryId);

            return View(product);
        }


        // Edit Product - GET
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories =
                new SelectList(
                    await _context.Categories.ToListAsync(),
                    "CategoryId",
                    "CategoryName",
                    product.CategoryId);

            return View(product);
        }


        // Edit Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(
    int id,
    Product product,
    IFormFile? ImageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct =
                    await _context.Products
                        .FirstOrDefaultAsync(
                            p => p.ProductId == id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.ProductName =
                    product.ProductName;

                existingProduct.Brand =
                    product.Brand;

                existingProduct.Description =
                    product.Description ?? "";
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "products"
                    );

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(ImageFile.FileName);

                    string filePath =
                        Path.Combine(
                            uploadsFolder,
                            uniqueFileName
                        );

                    using (var fileStream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    existingProduct.ImageUrl =
                        "/images/products/" + uniqueFileName;
                }

                existingProduct.Price =
                    product.Price;

                existingProduct.SuitableSkinType =
                    product.SuitableSkinType ?? "";

                existingProduct.IsActive =
                    product.IsActive;

                existingProduct.CategoryId =
                    product.CategoryId;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Product updated successfully.";

                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories =
                new SelectList(
                    await _context.Categories.ToListAsync(),
                    "CategoryId",
                    "CategoryName",
                    product.CategoryId);

            return View(product);
        }


        // Delete Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product =
                await _context.Products
                    .FirstOrDefaultAsync(
                        p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Product deleted successfully.";

            return RedirectToAction(nameof(Products));
        }


        // =====================================
        // CATEGORIES
        // =====================================

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return View(categories);
        }


        // Add Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(
            string categoryName,
            string description)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                TempData["ErrorMessage"] =
                    "Category name is required.";

                return RedirectToAction(nameof(Categories));
            }

            var category = new Category
            {
                CategoryName = categoryName,
                Description = description ?? ""
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Category added successfully.";

            return RedirectToAction(nameof(Categories));
        }


        // Edit Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(
            int categoryId,
            string categoryName,
            string description)
        {
            var category =
                await _context.Categories
                    .FirstOrDefaultAsync(
                        c => c.CategoryId == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName =
                categoryName;

            category.Description =
                description ?? "";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Category updated successfully.";

            return RedirectToAction(nameof(Categories));
        }


        // Delete Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category =
                await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(
                        c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products != null &&
                category.Products.Any())
            {
                TempData["ErrorMessage"] =
                    "This category cannot be deleted because it contains products.";

                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Category deleted successfully.";

            return RedirectToAction(nameof(Categories));
        }


        // =====================================
        // INGREDIENTS
        // =====================================

        public async Task<IActionResult> Ingredients()
        {
            var ingredients =
                await _context.Ingredients
                    .OrderBy(i => i.IngredientName)
                    .ToListAsync();

            return View(ingredients);
        }


        // Add Ingredient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIngredient(
            string ingredientName,
            string description,
            string purpose,
            string suitableSkinType)
        {
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                TempData["ErrorMessage"] =
                    "Ingredient name is required.";

                return RedirectToAction(nameof(Ingredients));
            }

            var ingredient = new Ingredient
            {
                IngredientName = ingredientName,
                Description = description ?? "",
                Purpose = purpose ?? "",
                SuitableSkinType = suitableSkinType ?? ""
            };

            _context.Ingredients.Add(ingredient);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Ingredient added successfully.";

            return RedirectToAction(nameof(Ingredients));
        }


        // Edit Ingredient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIngredient(
            int ingredientId,
            string ingredientName,
            string description,
            string purpose,
            string suitableSkinType)
        {
            var ingredient =
                await _context.Ingredients
                    .FirstOrDefaultAsync(
                        i => i.IngredientId == ingredientId);

            if (ingredient == null)
            {
                return NotFound();
            }

            ingredient.IngredientName =
                ingredientName;

            ingredient.Description =
                description ?? "";

            ingredient.Purpose =
                purpose ?? "";

            ingredient.SuitableSkinType =
                suitableSkinType ?? "";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Ingredient updated successfully.";

            return RedirectToAction(nameof(Ingredients));
        }


        // Delete Ingredient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient =
                await _context.Ingredients
                    .Include(i => i.ProductIngredients)
                    .FirstOrDefaultAsync(
                        i => i.IngredientId == id);

            if (ingredient == null)
            {
                return NotFound();
            }

            if (ingredient.ProductIngredients != null &&
                ingredient.ProductIngredients.Any())
            {
                TempData["ErrorMessage"] =
                    "This ingredient cannot be deleted because it is linked to products.";

                return RedirectToAction(nameof(Ingredients));
            }

            _context.Ingredients.Remove(ingredient);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Ingredient deleted successfully.";

            return RedirectToAction(nameof(Ingredients));
        }


        // =====================================
        // USERS
        // =====================================

        public async Task<IActionResult> Users()
        {
            var users =
                await _userManager.Users.ToListAsync();

            var userList =
                new List<dynamic>();

            foreach (var user in users)
            {
                var roles =
                    await _userManager.GetRolesAsync(user);

                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    Role = roles.FirstOrDefault()
                        ?? "No Role"
                });
            }

            return View(userList);
        }


        // =====================================
        // MAKEUP PRODUCTS
        // =====================================

        public async Task<IActionResult> MakeupProducts()
        {
            var products = await _context.MakeupProducts
                .OrderBy(m => m.ProductName)
                .ToListAsync();

            return View(products);
        }


        // Add Makeup Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMakeupProduct(
    string productName,
    string brand,
    string category,
    string description,
    string suitableSkinType,
    string finishType,
    bool isActive,
    IFormFile? ImageFile)
        {
            if (string.IsNullOrWhiteSpace(productName) ||
                string.IsNullOrWhiteSpace(brand))
            {
                TempData["ErrorMessage"] =
                    "Product name and brand are required.";

                return RedirectToAction(nameof(MakeupProducts));
            }

            string imageUrl = "";

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "makeup"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(ImageFile.FileName);

                string filePath =
                    Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                imageUrl =
                    "/images/makeup/" + uniqueFileName;
            }

            var product = new MakeupProduct
            {
                ProductName = productName,
                Brand = brand,
                Category = category ?? "",
                Description = description ?? "",
                SuitableSkinType = suitableSkinType ?? "",
                FinishType = finishType ?? "",
                ImageUrl = imageUrl,
                IsActive = isActive
            };

            _context.MakeupProducts.Add(product);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Makeup product added successfully.";

            return RedirectToAction(nameof(MakeupProducts));
        }

        // Edit Makeup Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMakeupProduct(
            int makeupProductId,
            string productName,
            string brand,
            string category,
            string description,
            string suitableSkinType,
            string finishType,
            bool isActive,
            IFormFile? ImageFile)
        {
            var product = await _context.MakeupProducts
                .FirstOrDefaultAsync(
                    m => m.MakeupProductId == makeupProductId);

            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(productName) ||
                string.IsNullOrWhiteSpace(brand))
            {
                TempData["ErrorMessage"] =
                    "Product name and brand are required.";

                return RedirectToAction(nameof(MakeupProducts));
            }

            product.ProductName = productName;
            product.Brand = brand;
            product.Category = category ?? "";
            product.Description = description ?? "";
            product.SuitableSkinType = suitableSkinType ?? "";
            product.FinishType = finishType ?? "";
            product.IsActive = isActive;

            // Keep the current image if no new image is selected.
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "makeup"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(ImageFile.FileName);

                string filePath =
                    Path.Combine(
                        uploadsFolder,
                        uniqueFileName
                    );

                using (var fileStream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                product.ImageUrl =
                    "/images/makeup/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Makeup product updated successfully.";

            return RedirectToAction(nameof(MakeupProducts));
        }


        // =====================================
        // ROUTINES
        // =====================================

        public async Task<IActionResult> Routines()
        {
            var routines = await _context.RoutineRecommendations
                .OrderBy(r => r.SkinType)
                .ThenBy(r => r.RoutineTime)
                .ThenBy(r => r.StepOrder)
                .ToListAsync();

            return View(routines);
        }


        // Add Routine Step
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoutine(
            string skinType,
            string skinGoal,
            string routineTime,
            int stepOrder,
            string stepName,
            string description)
        {
            if (string.IsNullOrWhiteSpace(skinType) ||
                string.IsNullOrWhiteSpace(skinGoal) ||
                string.IsNullOrWhiteSpace(routineTime) ||
                string.IsNullOrWhiteSpace(stepName))
            {
                TempData["ErrorMessage"] =
                    "Please complete all required routine fields.";

                return RedirectToAction(nameof(Routines));
            }

            var routine = new RoutineRecommendation
            {
                SkinType = skinType,
                SkinGoal = skinGoal,
                RoutineTime = routineTime,
                StepOrder = stepOrder,
                StepName = stepName,
                Description = description ?? ""
            };

            _context.RoutineRecommendations.Add(routine);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Routine step added successfully.";

            return RedirectToAction(nameof(Routines));
        }


        // Edit Routine Step
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoutine(
            int routineRecommendationId,
            string skinType,
            string skinGoal,
            string routineTime,
            int stepOrder,
            string stepName,
            string description)
        {
            var routine = await _context.RoutineRecommendations
                .FirstOrDefaultAsync(
                    r => r.RoutineRecommendationId ==
                         routineRecommendationId);

            if (routine == null)
            {
                return NotFound();
            }

            routine.SkinType = skinType;
            routine.SkinGoal = skinGoal;
            routine.RoutineTime = routineTime;
            routine.StepOrder = stepOrder;
            routine.StepName = stepName;
            routine.Description = description ?? "";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Routine step updated successfully.";

            return RedirectToAction(nameof(Routines));
        }


        // Delete Routine Step
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoutine(int id)
        {
            var routine = await _context.RoutineRecommendations
                .FirstOrDefaultAsync(
                    r => r.RoutineRecommendationId == id);

            if (routine == null)
            {
                return NotFound();
            }

            _context.RoutineRecommendations.Remove(routine);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Routine step deleted successfully.";

            return RedirectToAction(nameof(Routines));
        }
        // =====================================
        // ORDERS
        // =====================================

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        // =====================================
        // ORDER DETAILS
        // =====================================

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // =====================================
        // UPDATE ORDER STATUS
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var allowedStatuses = new[]
            {
        "Placed",
        "Processing",
        "Shipped",
        "Delivered",
        "Cancelled"
    };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest();
            }

            order.Status = status;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Order status updated successfully.";

            return RedirectToAction(
                nameof(OrderDetails),
                new { id = order.OrderId });
        }
        // =====================================
        // VIEW WEBSITE HOME
        // =====================================

        public IActionResult SiteHome()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}