using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;

namespace SereGlowy.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =====================================
        // CART PAGE
        // =====================================

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();

            return View(cartItems);
        }


        // =====================================
        // ADD TO CART
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = _userManager.GetUserId(User);

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productId &&
                    p.IsActive);

            if (product == null)
            {
                return NotFound();
            }


            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId);


            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId!,
                    ProductId = productId,
                    Quantity = 1,
                    AddedAt = DateTime.Now
                };

                _context.CartItems.Add(cartItem);
            }


            await _context.SaveChangesAsync();

            TempData["CartMessage"] =
                "Product added to your cart.";

            return RedirectToAction(nameof(Index));
        }


        // =====================================
        // INCREASE QUANTITY
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increase(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CartItemId == id &&
                    c.UserId == userId);

            if (item == null)
            {
                return NotFound();
            }


            item.Quantity++;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // =====================================
        // DECREASE QUANTITY
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decrease(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CartItemId == id &&
                    c.UserId == userId);

            if (item == null)
            {
                return NotFound();
            }


            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                _context.CartItems.Remove(item);
            }


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // =====================================
        // REMOVE FROM CART
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CartItemId == id &&
                    c.UserId == userId);

            if (item == null)
            {
                return NotFound();
            }


            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // =====================================
        // CHECKOUT PAGE
        // =====================================

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Total = cartItems.Sum(c =>
                (c.Product?.Price ?? 0) * c.Quantity);

            return View(cartItems);
        }


        // =====================================
        // PLACE ORDER
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            string fullName,
            string phone,
            string address)
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction(nameof(Index));
            }


            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address))
            {
                ViewBag.ErrorMessage =
                    "Please complete all delivery information.";

                ViewBag.Total = cartItems.Sum(c =>
                    (c.Product?.Price ?? 0) * c.Quantity);

                return View(cartItems);
            }


            decimal total = cartItems.Sum(c =>
                (c.Product?.Price ?? 0) * c.Quantity);


            var order = new Order
            {
                UserId = userId!,
                FullName = fullName,
                Phone = phone,
                Address = address,
                TotalAmount = total,
                Status = "Placed",
                OrderDate = DateTime.Now
            };


            foreach (var item in cartItems)
            {
                if (item.Product != null)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    });
                }
            }


            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();


            return RedirectToAction(
                nameof(OrderSuccess),
                new { id = order.OrderId });
        }


        // =====================================
        // ORDER SUCCESS
        // =====================================

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.OrderId == id &&
                    o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }

}