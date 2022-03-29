using System.Diagnostics;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Mvc;
using Status.WebApp.Models;

namespace Status.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Options _options;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _options = new Options();
    }

    public IActionResult Index()
    {
        return Redirect(_options.UIPath);
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
}

