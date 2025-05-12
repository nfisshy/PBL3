using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using Microsoft.AspNetCore.Http;

namespace PBL3.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Cart()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0) return RedirectToAction("Login", "Account");
            var cartGroups = _cartService.GetCart(buyerId);
            return View(cartGroups);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            _cartService.UpdateQuantity(buyerId, productId, quantity);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Remove([FromBody] int productId)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            _cartService.RemoveFromCart(buyerId, productId);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return Json(new { requireLogin = true });
            _cartService.AddToCart(buyerId, productId, quantity);
            return Json(new { success = true });
        }
    }
} 