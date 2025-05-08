using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL_3.Models;

namespace PBL_3.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
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
