using System.Text.Json;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers;
public class CompareController(AppDbContext db) : Controller
{
    private List<int> Ids() => JsonSerializer.Deserialize<List<int>>(HttpContext.Session.GetString("CompareIds") ?? "[]") ?? [];
    public async Task<IActionResult> Index()
    {
        var ids = Ids();
        return View(await db.Cars.AsNoTracking().Where(CarVisibility.Public).Where(x => ids.Contains(x.Id)).Include(x => x.CarModel).ThenInclude(x => x.Manufacturer).Include(x => x.Images).ToListAsync());
    }
    [HttpPost]
    public async Task<IActionResult> Add(int carId)
    {
        if (!await db.Cars.Where(CarVisibility.Public).AnyAsync(x => x.Id == carId)) return NotFound();
        var ids = Ids();
        if (!ids.Contains(carId)) { if (ids.Count >= 3) { TempData["Message"] = "Chỉ so sánh tối đa 3 xe. Hãy bỏ một xe trước."; return RedirectToAction(nameof(Index)); } ids.Add(carId); }
        HttpContext.Session.SetString("CompareIds", JsonSerializer.Serialize(ids)); return RedirectToAction(nameof(Index));
    }
    [HttpPost] public IActionResult Remove(int carId) { var ids = Ids(); ids.Remove(carId); HttpContext.Session.SetString("CompareIds", JsonSerializer.Serialize(ids)); return RedirectToAction(nameof(Index)); }
}
