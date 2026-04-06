using LuyenTap.Data;
using LuyenTap.Helpers;
using LuyenTap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenTap.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = CookieCartHelper.GetCart(Request);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = CookieCartHelper.GetCart(Request);
            var existing = cart.FirstOrDefault(x => x.ProductId == productId);

            if (existing == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.ModelName ?? product.ModelNumber ?? "San pham",
                    UnitPrice = product.UnitCost,
                    Quantity = quantity,
                    ImageUrl = product.ProductImage
                });
            }
            else
            {
                existing.Quantity += quantity;
            }

            CookieCartHelper.SaveCart(Response, cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            var cart = CookieCartHelper.GetCart(Request);
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                CookieCartHelper.SaveCart(Response, cart);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            CookieCartHelper.SaveCart(Response, new List<CartItem>());
            return RedirectToAction(nameof(Index));
        }
    }
}
