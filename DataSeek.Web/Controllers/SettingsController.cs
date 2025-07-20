using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DataSeek.Web.Models;
using DataSeek.Web.Service.Contract;

namespace DataSeek.Web.Controllers;

public class SettingsController : Controller
{
    private readonly ILogger<SettingsController> _logger;
    private readonly ISettingsService _settingsService;
    
    public SettingsController(ILogger<SettingsController> logger, ISettingsService service)
    {
        _logger = logger;
        _settingsService = service;
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
}