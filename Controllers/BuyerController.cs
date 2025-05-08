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
    }
} 