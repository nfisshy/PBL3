using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL_3.Models;
using PBL3.Services;
using PBL3.DTO.Buyer;
namespace PBL_3.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProductService _productService;

    public HomeController(ILogger<HomeController> logger, ProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public IActionResult Index()
    {
            try
            {
                var products = _productService.GetAllProducts();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách sản phẩm";
                return View(new List<Buyer_SanPhamDTO>());
            }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Login()
    {
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Register()
    {
        return RedirectToAction("Register", "Account");
    }
}
