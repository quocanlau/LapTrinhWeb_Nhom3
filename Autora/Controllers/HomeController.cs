using System.Diagnostics;
using Autora.Services;
using Autora.ViewModels;
using Autora.Models;
using Microsoft.AspNetCore.Mvc;

namespace Autora.Controllers;

public class HomeController(CarQuery query) : Controller
{
    public async Task<IActionResult> Index() => View(await query.ListAsync(new CarFilter { PageSize = 3 }, loadManufacturers: false));

    public IActionResult About()
    {
        ViewBag.Title = "Về Chúng Tôi";
        ViewBag.ActiveMenu = "About";
        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        if (culture is not ("vi" or "vi-VN" or "en" or "en-US")) return BadRequest("Ngôn ngữ không hợp lệ.");
        Response.Cookies.Append(
            Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.DefaultCookieName,
            Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.MakeCookieValue(new Microsoft.AspNetCore.Localization.RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
    public IActionResult Information(string topic) { if (topic is not ("installment" or "inspection")) return NotFound(); return View(model: topic); }
}