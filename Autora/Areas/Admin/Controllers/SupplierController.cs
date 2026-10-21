using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class SupplierController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? keyword = null)
    {
        var q = db.Suppliers.AsNoTracking(); if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(x => x.Name.Contains(keyword)); return View(await q.OrderBy(x => x.Name).ThenBy(x => x.Id).PageAsync(page, keyword));
    }
    public async Task<IActionResult> Edit(int id = 0) { if (id == 0) return View(new SupplierForm()); var s = await db.Suppliers.FindAsync(id); return s is null ? NotFound() : View(new SupplierForm { Id = s.Id, Name = s.Name, SupplierType = s.SupplierType, Phone = s.Phone, Address = s.Address, IsActive = s.IsActive }); }
    [HttpPost]
    public async Task<IActionResult> Save(SupplierForm f)
    {
        if (!ModelState.IsValid) return View("Edit", f); var s = f.Id == 0 ? new Supplier() : await db.Suppliers.FindAsync(f.Id); if (s is null) return NotFound();
        s.Name = f.Name.Trim(); s.SupplierType = f.SupplierType; s.Phone = f.Phone; s.Address = f.Address; s.IsActive = f.IsActive; if (f.Id == 0) db.Suppliers.Add(s); await db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpPost] public async Task<IActionResult> Delete(int id) { var s = await db.Suppliers.FindAsync(id); if (s is null) return NotFound(); if (await db.PurchaseReceipts.AnyAsync(x => x.SupplierId == id)) throw new BusinessException("Đối tác có chứng từ. Hãy ngừng sử dụng thay vì xóa."); db.Suppliers.Remove(s); await db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
}
