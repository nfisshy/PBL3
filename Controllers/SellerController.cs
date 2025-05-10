using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using PBL3.DTO.Seller;
using System;
using Microsoft.AspNetCore.Http;

namespace PBL_3.Controllers
{
    public class SellerController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly ILogger<SellerController> _logger;

        public SellerController(SellerService sellerService, ILogger<SellerController> logger)
        {
            _sellerService = sellerService;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra xem người bán đã hoàn thành thông tin chưa
                if (!_sellerService.IsSellerProfileComplete(sellerId.Value))
                {
                    return RedirectToAction("CompleteProfile");
                }

                var dashboardData = _sellerService.GetDashboardData(sellerId.Value);
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu dashboard");
                TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu dashboard";
                return View(new Seller_DashboardDTO());
            }
        }

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Nếu đã hoàn thành thông tin thì chuyển về dashboard
            if (_sellerService.IsSellerProfileComplete(sellerId.Value))
            {
                return RedirectToAction("Dashboard");
            }

            var model = _sellerService.GetSellerProfile(sellerId.Value);
            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteProfile(Seller_SignUpDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Use temporary address if available
                if (TempData["TempAddress"] != null)
                {
                model.AddressSeller = TempData.Peek("TempAddress")?.ToString();
                }

                _sellerService.UpdateSellerProfile(sellerId.Value, model);
                TempData["Success"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin người bán");
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult EditAddress()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get temporary address from TempData if it exists
            string tempAddress = TempData["TempAddress"]?.ToString();
            var model = _sellerService.GetSellerAddress(sellerId.Value, tempAddress);
            
            if (model == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người bán";
                return RedirectToAction("CompleteProfile");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult EditAddress(Seller_SignUpAdjustDTO model)
        {
            if (!ModelState.IsValid)
                return View("EditAddress", model);

            try
            {
                // Store address components in TempData instead of saving to database
                TempData["TempAddress"] = $"{model.DetailAddress}, {model.Commune}, {model.District}, {model.Provine}";
                TempData["Success"] = "Địa chỉ đã được cập nhật tạm thời";
                return RedirectToAction("CompleteProfile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật địa chỉ");
                ModelState.AddModelError("", ex.Message);
                return View("EditAddress", model);
            }
        }
    }
} 