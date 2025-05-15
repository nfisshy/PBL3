using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using PBL3.DTO.Buyer;
using PBL3.Enums;
using System;
using Microsoft.AspNetCore.Http;

namespace PBL_3.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // Trang chủ - Hiển thị tất cả sản phẩm với sidebar danh mục, header tài khoản, giỏ hàng
        public IActionResult Index()
        {
            try
            {
                var products = _productService.GetAllProducts();
                var categories = Enum.GetValues(typeof(TypeProduct)).Cast<TypeProduct>().ToList();
                string userName = HttpContext.Session.GetString("UserName");
                int cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;

                ViewBag.Categories = categories;
                ViewBag.UserName = userName;
                ViewBag.CartCount = cartCount;

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách sản phẩm";
                return View(new List<Buyer_SanPhamDTO>());
            }
        }

        // Xử lý nút tài khoản
        public IActionResult Account()
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");
            return RedirectToAction("ThongTinTaiKhoan", "Buyer");
        }

        // Xử lý nút giỏ hàng
        public IActionResult Cart()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToAction("Login", "Account");
            return RedirectToAction("Index", "Cart");
        }

        // Lọc sản phẩm theo danh mục
        public IActionResult Category(TypeProduct category)
        {
            try
            {
                var products = _productService.GetProductsByCategory(category);
                string userName = HttpContext.Session.GetString("UserName");
                int cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
                ViewBag.Categories = Enum.GetValues(typeof(TypeProduct)).Cast<TypeProduct>().ToList();
                ViewBag.UserName = userName;
                ViewBag.CartCount = cartCount;
                return View(products);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Danh mục không hợp lệ: {Category}", category);
                TempData["Error"] = "Danh mục sản phẩm không hợp lệ";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm theo danh mục {Category}", category);
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách sản phẩm";
                return RedirectToAction("Index");
            }
        }

        // Trang chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            try
            {
                var product = _productService.GetProductDetails(id);
                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction("Index");
                }

                // Get user authentication status
                int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
                string userName = HttpContext.Session.GetString("UserName");
                int cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;

                // Set layout based on authentication status
                ViewBag.Layout = (buyerId == 0)
                    ? "~/Views/Shared/Layout.cshtml"
                    : "~/Views/Shared/BuyerLayout.cshtml";

                // Pass necessary data to view
                ViewBag.Categories = Enum.GetValues(typeof(TypeProduct)).Cast<TypeProduct>().ToList();
                ViewBag.UserName = userName;
                ViewBag.CartCount = cartCount;
                ViewBag.IsAuthenticated = buyerId != 0;

                return View(product);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy sản phẩm ID: {ProductId}", id);
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết sản phẩm ID: {ProductId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin sản phẩm";
                return RedirectToAction("Index");
            }
        }

        // Tìm kiếm sản phẩm
        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return RedirectToAction("Index");
                }

                var products = _productService.GetAllProducts()
                    .Where(p => p.ProductName.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();

                string userName = HttpContext.Session.GetString("UserName");
                int cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
                ViewBag.Categories = Enum.GetValues(typeof(TypeProduct)).Cast<TypeProduct>().ToList();
                ViewBag.UserName = userName;
                ViewBag.CartCount = cartCount;
                ViewBag.SearchTerm = searchTerm;
                return View("Index", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm sản phẩm với từ khóa: {SearchTerm}", searchTerm);
                TempData["Error"] = "Có lỗi xảy ra khi tìm kiếm sản phẩm";
                return RedirectToAction("Index");
            }
        }
    }
} 