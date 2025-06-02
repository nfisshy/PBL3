using Microsoft.AspNetCore.Mvc;
using PBL3.DTO.Shared;
using PBL3.Services;
using System;
using Microsoft.AspNetCore.Http;

namespace PBL_3.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly CartService _cartService;
        private readonly BuyerService _buyerService;

        public AccountController(AccountService accountService, CartService cartService, BuyerService buyerService)
        {
            _accountService = accountService;
            _cartService = cartService;
            _buyerService = buyerService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDTO model, int? addToCartProductId, int? addToCartQuantity, string? saveVoucherId, int? sellerId, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = _accountService.Login(model);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.RoleName.ToString());

                if (user.RoleName.ToString() == "Buyer")
                {
                    if (!string.IsNullOrEmpty(saveVoucherId) && sellerId.HasValue)
                    {
                        try
                        {
                            _buyerService.SaveVoucherForBuyer(user.Id, saveVoucherId);
                            TempData["SaveVoucherSuccess"] = "Đã lưu voucher thành công!";
                            return RedirectToAction("ViewShop", "Seller", new { sellerId = sellerId.Value });
                        }
                        catch (Exception ex)
                        {
                            TempData["SaveVoucherError"] = "Lỗi khi lưu voucher: " + ex.Message;
                            return RedirectToAction("ViewShop", "Seller", new { sellerId = sellerId.Value });
                        }
                    }
                    if (addToCartProductId.HasValue && addToCartQuantity.HasValue)
                    {
                        _cartService.AddToCart(user.Id, addToCartProductId.Value, addToCartQuantity.Value);
                        TempData["AddToCartSuccess"] = true;
                        return RedirectToAction("Details", "Product", new { id = addToCartProductId.Value });
                    }

                    // Nếu có returnUrl, chuyển về
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Product");
                }
                else if (user.RoleName.ToString() == "Seller")
                {
                    return RedirectToAction("Dashboard", "Seller");
                }
                else if (user.RoleName.ToString() == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Vai trò người dùng không hợp lệ");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tài khoản hoặc mật khẩu không đúng"))
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
                else
                    ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = _accountService.Register(model);
                // Đăng ký xong tự động đăng nhập

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                // Nếu lỗi là do tài khoản đã tồn tại hoặc lỗi chung, hiển thị ở đầu form
                if (ex.Message.Contains("Tài khoản đã tồn tại") || ex.Message.Contains("Vai trò không hợp lệ"))
                    ModelState.AddModelError("", ex.Message);
                else
                    ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}