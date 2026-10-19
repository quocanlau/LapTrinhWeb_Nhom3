using Autora.Data;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers;
public class CarController(CarQuery query, AppDbContext db) : Controller
{
    public async Task<IActionResult> Index([FromQuery] CarFilter filter)
    {
        if (!ModelState.IsValid) return BadRequest("Bộ lọc không hợp lệ.");
        return View(await query.ListAsync(filter));
    }
    public async Task<IActionResult> List([FromQuery] CarFilter filter)
    {
        if (!ModelState.IsValid) return BadRequest("Bộ lọc không hợp lệ.");
        return PartialView("_CarResults", await query.ListAsync(filter, false));
    }
    public async Task<IActionResult> Details(int id)
    {
        var car = await db.Cars.AsNoTracking().Where(CarVisibility.Public).Include(x => x.CarModel).ThenInclude(x => x.Manufacturer).Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id, HttpContext.RequestAborted);
        if (car is null) return NotFound();
        const string key = "Autora_RecentlyViewedCars";
        var ids = RecentCarIds.Parse(Request.Cookies[key]);
        ViewBag.RecentCars = await db.Cars.AsNoTracking().Where(CarVisibility.Public).Where(x => ids.Contains(x.Id) && x.Id != id).Include(x => x.CarModel).ThenInclude(x => x.Manufacturer).Include(x => x.Images).Take(3).ToListAsync(HttpContext.RequestAborted);
        ids.Remove(id); ids.Insert(0, id);
        Response.Cookies.Append(key, string.Join(',', ids.Take(5)), new CookieOptions { HttpOnly = true, IsEssential = true, SameSite = SameSiteMode.Lax, Secure = Request.IsHttps, Expires = DateTimeOffset.UtcNow.AddDays(7) });
        return View(car);
    }
}
