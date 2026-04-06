using System.Text.Json;
using LuyenTap.Models;

namespace LuyenTap.Helpers
{
    public static class CookieCartHelper
    {
        public const string CartCookieName = "LT_CART";

        public static List<CartItem> GetCart(HttpRequest request)
        {
            if (!request.Cookies.TryGetValue(CartCookieName, out var json) || string.IsNullOrWhiteSpace(json))
            {
                return new List<CartItem>();
            }

            try
            {
                var cart = JsonSerializer.Deserialize<List<CartItem>>(json);
                return cart ?? new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
        }

        public static void SaveCart(HttpResponse response, List<CartItem> cart)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            var json = JsonSerializer.Serialize(cart);
            response.Cookies.Append(CartCookieName, json, options);
        }

        public static int GetCartQuantity(HttpRequest request)
        {
            return GetCart(request).Sum(x => x.Quantity);
        }
    }
}
