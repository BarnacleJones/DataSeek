using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DataSeek.Web.Models;
using DataSeek.Web.Service.Contract;

namespace DataSeek.Web.Controllers;

public class UploadController(ILogger<UploadController> logger, IUploadService service) : Controller
{
    private readonly ILogger<UploadController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("File", "Please select a file.");
            return View("Index");
        }

        await service.ProcessUploadAsync(file);
        return RedirectToAction("Index"); // or another page like Results
    }
}