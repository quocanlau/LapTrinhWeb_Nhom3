using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Admin)]
public class SettingController(AppDbContext db, ShowroomService showroom) : Controller
{
    public async Task<IActionResult> Index() => View(await showroom.GetAsync());
    [HttpPost]
    public async Task<IActionResult> SaveShowroom([Bind("Name,Hotline,Email,Address,OpeningHours,Policy")] ShowroomSetting f, string? version)
    {
        ModelState.Remove(nameof(ShowroomSetting.RowVersion)); if (!ModelState.IsValid) { ViewBag.Version = version; return View("Index", f); }
        var s = await db.ShowroomSettings.SingleOrDefaultAsync(x => x.Id == 1);
        if (s is null) { if (!string.IsNullOrEmpty(version)) throw new BusinessException("Cấu hình đã thay đổi. Tải lại trang."); s = new ShowroomSetting(); db.ShowroomSettings.Add(s); }
        else VersionGuard.Apply(db, s, version);
        s.Name = f.Name.Trim(); s.Hotline = f.Hotline.Trim(); s.Email = f.Email.Trim(); s.Address = f.Address.Trim(); s.OpeningHours = f.OpeningHours; s.Policy = f.Policy;
        await db.SaveChangesAsync(); TempData["Message"] = "Đã lưu cấu hình vào database."; return RedirectToAction(nameof(Index));
    }
}
