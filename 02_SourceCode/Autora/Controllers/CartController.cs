using System.Text.Json;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers;
public class CartController(AppDbContext db) : Controller
{
    private List<int> Ids() => JsonSerializer.Deserialize<List<int>>(HttpContext.Session.GetString("CartIds") ?? "[]") ?? [];
    public async Task<IActionResult> Index()
    {
        var ids = Ids();
        return View(await db.Cars.AsNoTracking()
            .Include(x => x.CarModel).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Images)
            .Where(CarVisibility.Public).Where(x => ids.Contains(x.Id))
            .ToListAsync());
    }
    [HttpPost]
    public async Task<IActionResult> Add(int carId)
    {
        if (!await db.Cars.Where(CarVisibility.Public).AnyAsync(x => x.Id == carId && x.Status == CarStatus.Available)) return Conflict(new { message = "Xe không còn nhận yêu cầu cọc mới." });
        var ids = Ids(); if (!ids.Contains(carId)) { if (ids.Count >= 20) return BadRequest(new { message = "Danh sách tối đa 20 xe." }); ids.Add(carId); }
        HttpContext.Session.SetString("CartIds", JsonSerializer.Serialize(ids)); return Json(new { count = ids.Count });
    }
    [HttpPost] public IActionResult Remove(int carId) { var ids = Ids(); ids.Remove(carId); HttpContext.Session.SetString("CartIds", JsonSerializer.Serialize(ids)); return RedirectToAction("Index"); }
}
