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

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = _accountService.Login(model);
                // Lưu thông tin vào session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);

                HttpContext.Session.SetString("Role", user.RoleName.ToString());    
                if(user.RoleName.ToString() == "Buyer"){
                    return RedirectToAction("Index", "Product");
                }
                else {
                    return RedirectToAction("Dashboard", "Seller");
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
    }
}