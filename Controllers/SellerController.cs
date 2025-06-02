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

        [HttpGet]
        public IActionResult VoucherList()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                var vouchers = _sellerService.GetVoucherList(sellerId.Value);
                return View(vouchers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult CreateVoucher()
        {
            return View(new Seller_TaoGiamGiaDTO());
        }

        [HttpPost]
        public IActionResult CreateVoucher(Seller_TaoGiamGiaDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var sellerId = HttpContext.Session.GetInt32("UserId");
                _sellerService.CreateVoucher(sellerId.Value, model);
                TempData["Success"] = "Tạo voucher thành công";
                return RedirectToAction(nameof(VoucherList));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult UpdateVoucherStatus(string voucherId, bool isActive)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                _sellerService.UpdateVoucherStatus(sellerId.Value, voucherId, isActive);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteVoucher(string voucherId)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                _sellerService.DeleteVoucher(sellerId.Value, voucherId);
                TempData["Success"] = "Xóa voucher thành công";
                return RedirectToAction(nameof(VoucherList));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(VoucherList));
            }
        }

        [HttpGet]
        public IActionResult OrderDetail(int id)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var orderDetail = _sellerService.GetOrderDetail(sellerId.Value, id);
                if (orderDetail == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction("OrderManage");
                }

                return View(orderDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết đơn hàng");
                TempData["Error"] = ex.Message;
                return RedirectToAction("OrderManage");
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrdStatus newStatus)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                _sellerService.UpdateOrderStatus(sellerId.Value, orderId, newStatus);
                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Profile()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (sellerId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var profile = _sellerService.GetSellerPersonalInfo(sellerId.Value);
                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult UpdateProfile(Seller_ThongTinCaNhanDTO profile, IFormFile avatarFile)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (sellerId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                byte[] newAvatar = null;
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(avatarFile.ContentType.ToLower()))
                    {
                        throw new Exception("Chỉ chấp nhận file ảnh (JPEG, PNG, GIF)");
                    }

                    // Validate file size (max 5MB)
                    if (avatarFile.Length > 5 * 1024 * 1024)
                    {
                        throw new Exception("Kích thước file không được vượt quá 5MB");
                    }

                    using (var ms = new MemoryStream())
                    {
                        avatarFile.CopyTo(ms);
                        newAvatar = ms.ToArray();
                    }
                }

                _sellerService.UpdateSellerProfile(sellerId.Value, profile, newAvatar);
                TempData["Success"] = "Cập nhật thông tin thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhau(Seller_DoiMatKhauDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _sellerService.DoiMatKhau(sellerId.Value, model);
                TempData["Success"] = "Đổi mật khẩu thành công";
                return RedirectToAction("Profile");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy seller ID: {SellerId}", sellerId);
                TempData["Error"] = "Không tìm thấy tài khoản người bán";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu seller ID: {SellerId}", sellerId);
                TempData["Error"] = "Có lỗi xảy ra khi đổi mật khẩu";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Wallet()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var walletInfo = _sellerService.GetWalletInfo(sellerId.Value);
                return View(walletInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin ví");
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin ví";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public IActionResult LinkBank()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Lấy thông tin ví để hiển thị thông tin ngân hàng hiện tại (nếu có)
                var walletInfo = _sellerService.GetWalletInfo(sellerId.Value);
                var model = new Seller_LienKetNganHangDTO
                {
                    BankName = walletInfo.BankName != "Chưa liên kết" ? walletInfo.BankName : "",
                    BankNumber = walletInfo.BankNumber != "Chưa liên kết" ? walletInfo.BankNumber : ""
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang liên kết ngân hàng");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang liên kết ngân hàng";
                return RedirectToAction("Wallet");
            }
        }

        [HttpPost]
        public IActionResult LinkBank(Seller_LienKetNganHangDTO model)
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

                _sellerService.LinkBankAccount(sellerId.Value, model);
                TempData["Success"] = "Liên kết ngân hàng thành công";
                return RedirectToAction("Wallet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi liên kết ngân hàng");
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult NapTien()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var walletInfo = _sellerService.GetWalletInfo(sellerId.Value);
                var model = new Seller_RutNapTienDTO
                {
                    WalletBalance = walletInfo.WalletBalance,
                    BankName = walletInfo.BankName,
                    BankNumber = walletInfo.BankNumber
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang nạp tiền");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang nạp tiền";
                return RedirectToAction("Wallet");
            }
        }

        [HttpPost]
        public IActionResult NapTien(Seller_RutNapTienDTO model)
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

                _sellerService.NapTien(sellerId.Value, model);
                TempData["Success"] = "Nạp tiền thành công";
                return RedirectToAction("Wallet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi nạp tiền");
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult RutTien()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var walletInfo = _sellerService.GetWalletInfo(sellerId.Value);
                var model = new Seller_RutNapTienDTO
                {
                    WalletBalance = walletInfo.WalletBalance,
                    BankName = walletInfo.BankName,
                    BankNumber = walletInfo.BankNumber
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang rút tiền");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang rút tiền";
                return RedirectToAction("Wallet");
            }
        }

        [HttpPost]
        public IActionResult RutTien(Seller_RutNapTienDTO model)
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

                _sellerService.RutTien(sellerId.Value, model);
                TempData["Success"] = "Rút tiền thành công";
                return RedirectToAction("Wallet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi rút tiền");
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult CreatePin()
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var pinInfo = _sellerService.GetPinStatus(sellerId.Value);

                return View(pinInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang tạo PIN");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang tạo PIN";
                return RedirectToAction("Wallet");
            }
        }
        [HttpPost]
        public IActionResult CreatePin(Seller_TaoPinDTO model)
        {

            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _sellerService.UpdatePin(sellerId.Value, model);
                TempData["Success"] = "Cập nhật PIN thành công";
                return RedirectToAction("Wallet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo PIN");
                TempData["Error"] = ex.Message;
                var pinInfo = _sellerService.GetPinStatus(sellerId.Value);
                return View(pinInfo);
            }
        }

        [HttpGet]
        public IActionResult ViewShop(int sellerId)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                ViewBag.Layout = (currentUserId == 0)
                    ? "~/Views/Shared/Layout.cshtml"
                    : "~/Views/Shared/BuyerLayout.cshtml";

                // Lấy thông tin cửa hàng
                var shopInfo = _sellerService.ViewShop(sellerId);
                if (shopInfo == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin cửa hàng";
                    return RedirectToAction("Index", "Product");
                }

                return View(shopInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem thông tin cửa hàng");
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin cửa hàng";
                return RedirectToAction("Index", "Product");
            }
        }

        public IActionResult EditProduct(int id)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                var product = _sellerService.GetProductDetail(sellerId.Value, id);
                if (product == null)
                {
                    return NotFound();
                }

                var editModel = new EditProductDTO
                {
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ProductQuantity = product.InitialQuantity,
                    Description = product.Description,
                    CurrentImage = product.Image
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ProductManagement");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(int id, EditProductDTO model, IFormFile ProductImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Xử lý upload ảnh
                if (ProductImageFile != null && ProductImageFile.Length > 0)
                {
                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(ProductImageFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("ProductImage", "Chỉ chấp nhận file ảnh (JPEG, PNG, GIF)");
                        return View(model);
                    }

                    // Validate file size (max 5MB)
                    if (ProductImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ProductImage", "Kích thước file không được vượt quá 5MB");
                        return View(model);
                    }

                    // Đọc file thành byte array
                    using (var stream = ProductImageFile.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            model.ProductImage = ms.ToArray();
                        }
                    }
                }
                else
                {
                    // Nếu không có ảnh mới, giữ nguyên ảnh cũ
                    model.ProductImage = model.CurrentImage;
                }

                _sellerService.UpdateProduct(sellerId.Value, id, model);

                TempData["Success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("ProductDetail", new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult ReturnExchangeManagement(DateTime? fromDate, DateTime? toDate, ExchangeStatus? status)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var list = _sellerService.GetAllBySeller(sellerId.Value, fromDate, toDate, status);
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                ViewBag.SelectedStatus = status;
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách đổi trả");
                TempData["Error"] = "Đã xảy ra lỗi khi tải danh sách đổi trả.";
                return View(new List<Seller_ReturnExchangeDTO>());
            }
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var success = _sellerService.UpdateStatus(id, ExchangeStatus.Approved);
                if (!success)
                {
                    return Json(new { success = false, message = "Không thể duyệt yêu cầu đổi trả" });
                }

                return Json(new { success = true, message = "Đã duyệt yêu cầu đổi trả" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi duyệt yêu cầu đổi trả");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi duyệt yêu cầu" });
            }
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            try
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (!sellerId.HasValue)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var success = _sellerService.UpdateStatus(id, ExchangeStatus.Rejected);
                if (!success)
                {
                    return Json(new { success = false, message = "Không thể từ chối yêu cầu đổi trả" });
                }

                return Json(new { success = true, message = "Đã từ chối yêu cầu đổi trả" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi từ chối yêu cầu đổi trả");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi từ chối yêu cầu" });
            }
        }
    }
} 