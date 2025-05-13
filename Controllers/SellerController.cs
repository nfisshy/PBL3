using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using PBL3.DTO.Seller;
using PBL3.Enums;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;

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

            var model = new Seller_SignUpDTO();

            // Nếu có địa chỉ trong session thì gán vào model
            var tempAddress = HttpContext.Session.GetString("TempAddress");
            if (!string.IsNullOrEmpty(tempAddress))
            {
                model.AddressSeller = tempAddress;
            }

            return View(model);
        }
        

        [HttpPost]
        public IActionResult CompleteProfile(Seller_SignUpDTO model)
        {
            _logger.LogInformation("=== Đã vào CompleteProfile POST ==="); // dùng để check point
            var tempAddress = HttpContext.Session.GetString("TempAddress");
            if (!string.IsNullOrEmpty(tempAddress))
            {
                model.AddressSeller = tempAddress;
                ModelState.Remove(nameof(model.AddressSeller)); // Quan trọng: xóa lỗi cũ
            }
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogWarning($"❌ Lỗi ở trường {entry.Key}: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Use temporary address if available

                _logger.LogInformation($"Đang cập nhật: {model.StoreName}, {model.EmailGeneral}, {model.AddressSeller}");
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
            string tempAddress = HttpContext.Session.GetString("TempAddress");
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
                HttpContext.Session.SetString("TempAddress", $"{model.DetailAddress}, {model.Commune}, {model.District}, {model.Provine}");
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

        [HttpGet]
        public IActionResult OrderManage(DateTime? StartDate, DateTime? EndDate, OrdStatus? OrderStatus, int? OrderId)
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = _sellerService.GetOrderManagement(sellerId.Value, StartDate, EndDate, OrderStatus, OrderId);
            return View(model);
        }

        [HttpGet]
        public IActionResult ProductManagement(int? productId, string productName, PBL3.Enums.TypeProduct? productType)
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var products = _sellerService.GetProductList(sellerId.Value);

            // Lọc sản phẩm theo các điều kiện tìm kiếm
            if (productId.HasValue)
            {
                products = products.Where(p => p.ProductId == productId.Value).ToList();
            }
            if (!string.IsNullOrWhiteSpace(productName))
            {
                products = products.Where(p => p.ProductName.ToLower().Contains(productName.ToLower())).ToList();
            }
            if (productType.HasValue)
            {
                products = products.Where(p => p.TypeProduct == productType.Value).ToList();
            }

            // Lưu các giá trị tìm kiếm vào ViewBag để giữ lại trên form
            ViewBag.ProductId = productId; // viewbag chỉ có vòng đời tồn tại trong 1 Http request
            ViewBag.ProductName = productName;
            ViewBag.ProductType = productType;

            return View(products);
        }

        [HttpGet]
        public IActionResult ProductDetail(int productId)
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _sellerService.GetProductDetail(sellerId.Value, productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("ProductManagement");
            }

            return View(product);
        }

        // Add notification endpoints
        [HttpGet]
        public IActionResult Notifications()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var notifications = _sellerService.GetNewOrders(sellerId.Value);
            return View(notifications);
        }

        [HttpGet]
        public IActionResult GetNewOrdersCount()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return Json(new { count = 0 });
            }

            var count = _sellerService.GetNewOrdersCount(sellerId.Value);
            return Json(new { count });
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new CreateProductDTO());
        }

        [HttpPost]
        public IActionResult CreateProduct(CreateProductDTO model, IFormFile ProductImageFile)
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Xử lý upload ảnh
                if (ProductImageFile != null && ProductImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream()) // sau khi ra khỏi khối using đối tượng ms sẽ bị giải phóng
                    {
                        ProductImageFile.CopyTo(ms);
                        model.ProductImage = ms.ToArray();
                    }
                    ModelState.Remove("ProductImage");
                }
                else
                {
                    ModelState.AddModelError("ProductImage", "Vui lòng chọn hình ảnh sản phẩm");
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _sellerService.CreateProduct(sellerId.Value, model);
                TempData["Success"] = "Tạo sản phẩm thành công";
                return RedirectToAction("ProductManagement");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm mới");
                TempData["Error"] = "Có lỗi xảy ra khi tạo sản phẩm";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Statistics(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Set default date range if not provided
                if (!startDate.HasValue)
                    startDate = DateTime.Now.AddDays(-30); // Default to last 30 days
                if (!endDate.HasValue)
                    endDate = DateTime.Now;

                // Validate date range
                if (startDate > endDate)
                {
                    TempData["Error"] = "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc";
                    return View(new Seller_ThongKeDTO());
                }

                var statistics = _sellerService.GetStatistics(sellerId.Value, startDate.Value, endDate.Value);
                return View(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu thống kê");
                TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu thống kê";
                return View(new Seller_ThongKeDTO());
            }
        }

        [HttpGet]
        public IActionResult ProductReviews(int? productId)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var reviews = _sellerService.GetProductReviews(sellerId.Value, productId);
                
                // Lưu productId vào ViewBag để giữ lại trên form
                ViewBag.ProductId = productId;
                
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đánh giá");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách đánh giá";
                return View(new List<Seller_DanhGiaDTO>());
            }
        }
    }
} 