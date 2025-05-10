using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using PBL3.DTO.Buyer;
using PBL3.Enums;
using System;
using Microsoft.AspNetCore.Http;

namespace PBL3.Controllers
{
    public class BuyerController : Controller
    {
        private readonly BuyerService _buyerService;
        private readonly ILogger<BuyerController> _logger;
        public BuyerController(BuyerService buyerService, ILogger<BuyerController> logger)
        {
            _buyerService = buyerService;
            _logger = logger;
        }

        // Trang thông tin tài khoản buyer
        public IActionResult ThongTinTaiKhoan()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            try
            {
                var model = _buyerService.GetThongTinCaNhan(buyerId);
                return View(model);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID người mua không hợp lệ: {BuyerId}", buyerId);
                TempData["Error"] = "ID người mua không hợp lệ";
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Không tìm thấy người mua";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin tài khoản buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin tài khoản";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhau(Buyer_DoiMatKhauDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return RedirectToAction("Login", "Account");

            try
            {
                _buyerService.DoiMatKhau(buyerId, model);
                TempData["Success"] = "Đổi mật khẩu thành công";
                return RedirectToAction("ThongTinTaiKhoan");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Không tìm thấy người mua";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Có lỗi xảy ra khi đổi mật khẩu";
                return View(model);
            }
        }

        public IActionResult ThongBao()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return RedirectToAction("Login", "Account");

            try
            {
                var (donHang, voucher) = _buyerService.GetThongBao(buyerId);
                var viewModel = new ThongBaoViewModel
                {
                    DonHang = donHang,
                    Voucher = voucher
                };
                return View(viewModel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID người mua không hợp lệ: {BuyerId}", buyerId);
                TempData["Error"] = "ID người mua không hợp lệ";
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Không tìm thấy người mua";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Có lỗi xảy ra khi tải thông báo";
                return RedirectToAction("Index", "Home");
            }
        }
    }
} 