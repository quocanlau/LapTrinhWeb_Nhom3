using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=Roles.Staff)]
public class ManufacturerController(AppDbContext db):Controller
{
    public async Task<IActionResult> Index(){ViewBag.Models=await db.CarModels.AsNoTracking().Include(x=>x.Manufacturer).OrderBy(x=>x.Name).ToListAsync();return View(await db.Manufacturers.AsNoTracking().OrderBy(x=>x.Name).ToListAsync());}
    [HttpPost] public async Task<IActionResult> Save([Bind("Id,Name,Country,IsActive")]Manufacturer f)
    {
        if(!ModelState.IsValid)throw new BusinessException("Thông tin hãng không hợp lệ.");
        var m=f.Id==0?new Manufacturer():await db.Manufacturers.FindAsync(f.Id);if(m is null)return NotFound();
        m.Name=f.Name.Trim();m.Country=f.Country;m.IsActive=f.IsActive;if(f.Id==0)db.Manufacturers.Add(m);await db.SaveChangesAsync();return RedirectToAction("Index");
    }
    [HttpPost] public async Task<IActionResult> Delete(int id){var m=await db.Manufacturers.FindAsync(id);if(m is null)return NotFound();if(await db.CarModels.AnyAsync(x=>x.ManufacturerId==id))throw new BusinessException("Hãng đã có dòng xe. Hãy ngừng sử dụng thay vì xóa.");db.Manufacturers.Remove(m);await db.SaveChangesAsync();return RedirectToAction("Index");}
    [HttpPost] public async Task<IActionResult> AddModel([Bind("ManufacturerId,Name,BodyType")]CarModel model){ModelState.Remove(nameof(CarModel.Manufacturer));if(!ModelState.IsValid || !await db.Manufacturers.AnyAsync(x=>x.Id==model.ManufacturerId && x.IsActive))throw new BusinessException("Dòng xe không hợp lệ.");db.CarModels.Add(model);await db.SaveChangesAsync();return RedirectToAction("Index");}
}
