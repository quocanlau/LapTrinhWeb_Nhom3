using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers;
public class CarController(CarQuery query,AppDbContext db):Controller
{
    public async Task<IActionResult> Index([FromQuery]CarFilter filter)=>View(await query.ListAsync(filter));
    public async Task<IActionResult> List([FromQuery]CarFilter filter)=>PartialView("_CarResults",await query.ListAsync(filter, loadManufacturers: false));
    // Controllers/CarController.cs
    public async Task<IActionResult> Details(int id)
    {
        var car = await db.Cars.AsNoTracking()
            .Include(x => x.CarModel).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Images)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsPublished &&
                                  x.Status != CarStatus.AwaitingImport &&
                                  x.Status != CarStatus.Sold &&
                                  x.CarModel.IsActive &&
                                  x.CarModel.Manufacturer.IsActive);

        if (car is null) return NotFound();

        // === GHI COOKIE: LƯU DANH SÁCH XE VỪA XEM GẦN ĐÂY ===
        var cookieKey = "Autora_RecentlyViewedCars";
        var viewedIds = new List<int>();
        if (Request.Cookies.TryGetValue(cookieKey, out var existingValue) && !string.IsNullOrEmpty(existingValue))
        {
            try { viewedIds = existingValue.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(); }
            catch { viewedIds = new List<int>(); }
        }

        viewedIds.Remove(id);
        viewedIds.Insert(0, id); // Đưa xe hiện tại lên đầu
        var newCookieValue = string.Join(",", viewedIds.Take(5)); // Lưu tối đa 5 xe

        Response.Cookies.Append(cookieKey, newCookieValue, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

        return View(car);
    }
}
