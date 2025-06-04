using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using PBL3.DTO.Buyer;
using PBL3.Enums;
using PBL3.Repositories;
using System;
using Microsoft.AspNetCore.Http;

namespace PBL3.Controllers
{
    public class BuyerController : Controller
    {
        private readonly BuyerService _buyerService;
        private readonly ProductService _productService;
        private readonly ILogger<BuyerController> _logger;
        private readonly ReviewService _reviewService;
        private readonly IProductRepositories _productRepository;
        private readonly WalletService _walletService;
        private readonly ReturnExchangeService _returnExchangeService;
        public BuyerController(BuyerService buyerService, ILogger<BuyerController> logger,
                                ProductService productService, ReviewService reviewService,
                                IProductRepositories productRepository,
                                WalletService walletService, ReturnExchangeService returnExchangeService)
        {
            _buyerService = buyerService;
            _logger = logger;
            _productService = productService;
            _reviewService = reviewService;
            _productRepository = productRepository;
            _walletService = walletService;
            _returnExchangeService = returnExchangeService;
        }

        public IActionResult Index()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            try
            {
                var products = _productService.GetAllProducts();
                var categories = Enum.GetValues(typeof(TypeProduct)).Cast<TypeProduct>().ToList();
                string userName = HttpContext.Session.GetString("UserName");
                ViewBag.Categories = categories;
                ViewBag.UserName = userName;

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách sản phẩm";
                return View(new List<Buyer_SanPhamDTO>());
            }
        }
        // Trang thông tin tài khoản buyer
        [HttpGet]
        public IActionResult ThongTinTaiKhoan()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
        [HttpPost]
        public IActionResult UpdateProfile([FromBody] Buyer_ThongTinCaNhanDTO model)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0) return Json(new { success = false, message = "Chưa đăng nhập" });
            try
            {
                _buyerService.UpdateName(buyerId, model.Name);
                DateTime date;
                if (DateTime.TryParse(model.Date.ToString(), out date))
                    _buyerService.UpdateDate(buyerId, date);
                // if(Enum.TryParse(typeof(PBL3.Enums.Gender), model.Sex.ToString(), out var gender))
                //     _buyerService.UpdateSex(buyerId, (PBL3.Enums.Gender)gender);
                _buyerService.UpdatePhoneNumber(buyerId, model.PhoneNumber);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult AddressHome()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var addresses = _buyerService.GetAllAddressByBuyerId(buyerId);
            return View(addresses);
        }

        public ActionResult AddAddress()
        {

            int? buyerId = HttpContext.Session.GetInt32("UserId");

            if (buyerId == null || buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new Buyer_SoDiaChiDTO
            {
                BuyerId = buyerId.Value
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAddress(Buyer_SoDiaChiDTO dto)
        {
            int? buyerId = HttpContext.Session.GetInt32("UserId");

            if (buyerId == null || buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            dto.BuyerId = buyerId.Value;

            // if (!ModelState.IsValid)
            // {
            //     return View("AddAddress",dto);
            // }
            try
            {
                _buyerService.AddAddress(dto);
                return RedirectToAction("AddressHome");
            }
            catch (ArgumentNullException ex)
            {
                // Lỗi DTO null
                ModelState.AddModelError("", "Dữ liệu địa chỉ không được để trống.");
            }
            catch (ArgumentException ex)
            {
                // Bắt lỗi cụ thể theo tên trường bị lỗi
                switch (ex.ParamName)
                {
                    case "Ward":
                        ModelState.AddModelError("Ward", ex.Message);
                        break;
                    case "District":
                        ModelState.AddModelError("District", ex.Message);
                        break;
                    case "City":
                        ModelState.AddModelError("City", ex.Message);
                        break;
                    default:
                        ModelState.AddModelError("", ex.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm địa chỉ: " + ex.Message);
            }

            return View("AddAddress", dto);
        }

        [HttpPost]
        public JsonResult DeleteAddress([FromBody] int addressId)
        {
            int? buyerId = HttpContext.Session.GetInt32("UserId");

            if (buyerId == null || buyerId == 0)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            try
            {
                _buyerService.DeleteAddress(addressId);
                return Json(new { success = true, message = "Xóa địa chỉ thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult EditAddress(int addressId)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy địa chỉ đã tách sẵn từ service
            var dto = _buyerService.GetAddressById(addressId);

            // Kiểm tra địa chỉ có tồn tại và thuộc về buyer hiện tại không
            if (dto == null || dto.BuyerId != buyerId)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tìm địa chỉ";
                return RedirectToAction("AddressHome");
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress(Buyer_SoDiaChiDTO dto)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            dto.BuyerId = buyerId;
            try
            {
                _buyerService.UpdateAddress(dto);
                return RedirectToAction("AddressHome");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                // Nếu cần load lại dữ liệu để hiển thị (ví dụ dropdown, mặc định,...), xử lý ở đây

                return View(dto);
            }
        }

        public IActionResult ReviewHome()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            try
            {
                var reviews = _reviewService.GetAllReviewsByBuyer(buyerId);
                return View(reviews); // View `ReviewHome.cshtml` nhận List<Buyer_DanhGiaDTO>
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đánh giá");
                TempData["Error"] = "Không thể tải đánh giá.";
                return View(new List<Buyer_DanhGiaDTO>());
            }
        }

        [HttpGet]
        public IActionResult EditComment(int reviewId)
        {
            var dto = _reviewService.GetReviewById(reviewId);
            if (dto == null) return NotFound();

            return PartialView("_EditComment", dto); // _EditComment.cshtml là popup sửa đánh giá
        }

        // POST: Cập nhật đánh giá
        [HttpPost]
        public IActionResult EditComment([FromBody] Buyer_DanhGiaDTO dto)
        {
            _logger.LogInformation("Đã bấm vào nút sửa đánh giá");
            _logger.LogInformation("EditComment called with reviewId: {ReviewId}, content: {Content}, rating: {Rating}",
                dto.ReviewId, dto.Content, dto.Rating);

            try
            {
                _reviewService.UpdateReview(dto.ReviewId, dto.Content, dto.Rating);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sửa đánh giá");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult WriteComment(int productId)
        {
            var product = _productRepository.GetById(productId);
            if (product == null) return NotFound();

            var dto = new Buyer_DanhGiaDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductImage = product.ProductImage
            };
            return PartialView("_WriteComment", dto); // _WriteComment.cshtml là popup viết đánh giá
        }

        // POST: Gửi comment
        [HttpPost]
        public IActionResult WriteComment([FromBody] Buyer_DanhGiaDTO dto)
        {
            dto.BuyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            try
            {
                _reviewService.AddReview(dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm đánh giá");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult WalletHome()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToAction("Login", "Account");
            if (TempData["PinVerified"] != null)
            {
                var wallet = _walletService.OpenWallet(userId);
                return View("WalletHome", wallet);
            }
            bool isInitialized = _walletService.IsWalletInitialized(userId);
            ViewBag.IsWalletInitialized = isInitialized;

            return View(); // View trắng ban đầu, sẽ hiện popup yêu cầu nhập/tạo PIN
        }

        // Thiết lập mã PIN lần đầu
        [HttpPost]
        public IActionResult SetWalletPin(int newPin)
        {
            string str = newPin.ToString();
            if (str.Length != 6 || !str.All(char.IsDigit))
            {
                TempData["Error"] = "Mã PIN phải gồm đúng 6 chữ số.";
                return RedirectToAction("WalletHome");
            }
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return Unauthorized();
            bool result = _walletService.InitializeWalletPin(userId, newPin);
            if (!result)
            {
                TempData["Error"] = "Không thể thiết lập mã PIN. Có thể bạn đã thiết lập trước đó.";
                return RedirectToAction("WalletHome");
            }

            TempData["Success"] = "Thiết lập mã PIN thành công. Vui lòng đăng nhập ví.";
            return RedirectToAction("WalletHome");
        }

        // Mở ví bằng mã PIN
        [HttpPost]
        public IActionResult SubmitPin(int pin)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return Unauthorized();

            int attempts = HttpContext.Session.GetInt32("WalletLoginAttempts") ?? 0;

            var wallet = _walletService.OpenWallet(userId, pin);
            if (wallet == null)
            {
                attempts++;
                HttpContext.Session.SetInt32("WalletLoginAttempts", attempts);

                if (attempts >= 5)
                {
                    TempData["Error"] = "Bạn đã nhập sai mã PIN quá 5 lần.";
                    HttpContext.Session.Remove("WalletLoginAttempts");
                    return RedirectToAction("Index");
                }

                TempData["Error"] = $"Mã PIN không đúng. Lần sai thứ {attempts}/5.";
                return RedirectToAction("WalletHome");
            }

            HttpContext.Session.Remove("WalletLoginAttempts");
            return View("WalletHome", wallet);
        }

        // Nạp tiền
        [HttpPost]
        public IActionResult Recharge(decimal amount)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return Unauthorized();

            bool result = _walletService.Recharge(userId, amount);
            if (!result)
            {
                TempData["Error"] = "Nạp tiền thất bại.";
            }
            else
            {
                TempData["Success"] = $"Đã nạp {amount:N0} VNĐ vào ví.";
                TempData["PinVerified"] = true; // Gắn cờ không cần nhập lại PIN
            }

            return RedirectToAction("WalletHome");
        }

        [HttpPost]
        public IActionResult AddBank(string bankName, string accountNumber)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return Unauthorized();

            try
            {
                _walletService.AddBank(userId, bankName, accountNumber);

                TempData["Success"] = "Thêm tài khoản ngân hàng thành công.";
                TempData["PinVerified"] = true;
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message; // Lỗi do thiếu thông tin hoặc không tìm thấy ví
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message; // Lỗi trùng tài khoản ngân hàng
            }
            catch (Exception)
            {
                TempData["Error"] = "Đã xảy ra lỗi không xác định khi thêm tài khoản ngân hàng.";
            }

            return RedirectToAction("WalletHome");
        }


        // Đổi mã PIN
        [HttpPost]
        public IActionResult ChangeWalletPin(int oldPin, int newPin)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return Unauthorized();
            string str = newPin.ToString();
            if (str.Length != 6 || !str.All(char.IsDigit))
            {
                TempData["Error"] = "Cả mã PIN cũ và mới đều phải gồm đúng 6 chữ số.";
                return RedirectToAction("WalletHome");
            }
            bool result = _walletService.ChangeWalletPin(userId, oldPin, newPin);
            if (!result)
            {
                TempData["Error"] = "Mã PIN cũ không đúng. Không thể đổi mã PIN.";
            }
            else
            {
                TempData["Success"] = "Đổi mã PIN thành công.";
            }

            return RedirectToAction("WalletHome");
        }

        public IActionResult VoucherHome()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return RedirectToAction("Login", "Account");
            try
            {


                List<Buyer_VoucherDTO> vouchers = _buyerService.GetVouchersByBuyerId(buyerId);
                return View("VoucherHome", vouchers);
            }
            catch (Exception ex)
            {
                // Có thể log lỗi tại đây nếu cần
                return BadRequest("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ReturnExchangeHome()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                var allExchanges = _returnExchangeService.GetAll(buyerId);
                return View("ReturnExchangeHome", allExchanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đổi trả");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách yêu cầu đổi trả";
                return View("ReturnExchangeHome", new List<ExchangeStuffDTO>());
            }
        }



        // 2. Xem chi tiết yêu cầu đổi trả
        [HttpGet]
        public IActionResult ReturnExchangeDetail(int id)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                var detail = _returnExchangeService.GetById(id, buyerId);
                if (detail == null)
                {
                    TempData["Error"] = "Không tìm thấy yêu cầu đổi trả";
                    return RedirectToAction("ReturnExchangeHome");
                }

                return View("ReturnExchangeDetail", detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết yêu cầu đổi trả ID: {Id}", id);
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin yêu cầu";
                return RedirectToAction("ReturnExchangeHome");
            }
        }


        // 3. Thêm yêu cầu đổi trả (POST)
        [HttpPost]
        public IActionResult AddReturnExchange(ExchangeStuffDTO dto)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            dto.BuyerId = buyerId;
            dto.RequestDate = DateTime.Now;
            dto.ResponseDate = new DateTime(2001, 1, 1);
            dto.Status = ExchangeStatus.WaitConfirm;

            // Nếu có file ảnh được upload, chuyển sang byte[]
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    dto.ImageFile.CopyTo(ms);
                    dto.Image = ms.ToArray(); // Gán vào byte[] Image
                }
            }

            try
            {
                _returnExchangeService.Add(dto); // Thêm yêu cầu vào hệ thống
                TempData["Success"] = "Gửi yêu cầu đổi/trả thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm yêu cầu đổi/trả");
                TempData["Error"] = "Có lỗi xảy ra khi gửi yêu cầu đổi trả.";
            }

            return RedirectToAction("ReturnExchangeHome");
        }




        [HttpGet]
        public IActionResult AddReturnExchange(int productId, int orderId)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }


            var product = _productRepository.GetById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var dto = new ExchangeStuffDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductImage = product.ProductImage, // byte[]
                OrderId = orderId,
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult SaveVoucher(string voucherId)
        {
            try
            {
                int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (buyerId == 0)
                {
                    return Json(new { requireLogin = true });
                }

                _buyerService.SaveVoucherForBuyer(buyerId, voucherId);

                return Json(new { success = true, message = "Đã lưu voucher thành công!" });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu voucher." });
            }
        }
        
        [HttpPost]
        public IActionResult MarkVoucherAsUsed(string voucherId)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                _buyerService.UpdateVoucherIsUsed(buyerId, voucherId);
                return Json(new { success = true, message = "Voucher đã được đánh dấu là đã sử dụng." });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật voucher." });
            }
        }

    }
} 