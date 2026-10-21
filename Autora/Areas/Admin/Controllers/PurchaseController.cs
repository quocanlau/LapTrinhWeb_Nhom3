using System.Security.Claims;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class PurchaseController(AppDbContext db, PurchaseService service) : Controller
{
    public async Task<IActionResult> Index(int page = 1) => View(await db.PurchaseReceipts.AsNoTracking().Include(x => x.Supplier).Include(x => x.Details).ThenInclude(x => x.Car).ThenInclude(x => x.CarModel).OrderByDescending(x => x.Id).PageAsync(page));
    private async Task Choices(int id)
    {
        ViewBag.Suppliers = await db.Suppliers.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();
        ViewBag.Cars = await db.Cars.AsNoTracking().Where(x => x.Status == CarStatus.AwaitingImport && !db.PurchaseDetails.Any(d => d.CarId == x.Id && d.PurchaseReceiptId != id)).Include(x => x.CarModel).OrderBy(x => x.Id).ToListAsync();
    }
    public async Task<IActionResult> Edit(int id = 0)
    {
        await Choices(id); if (id == 0) return View(new PurchaseDraftForm());
        var p = await db.PurchaseReceipts.Include(x => x.Details).SingleOrDefaultAsync(x => x.Id == id); if (p is null) return NotFound(); if (p.Status != PurchaseStatus.Draft) throw new BusinessException("Phiếu đã xác nhận.");
        return View(new PurchaseDraftForm { Id = p.Id, SupplierId = p.SupplierId, Version = Convert.ToBase64String(p.RowVersion), Lines = p.Details.Select(x => new PurchaseLineForm { CarId = x.CarId, PurchasePrice = x.PurchasePrice }).ToList() });
    }
    [HttpPost] public async Task<IActionResult> Save(PurchaseDraftForm f) { if (!ModelState.IsValid) { await Choices(f.Id); return View("Edit", f); } await service.SaveDraftAsync(f, User.FindFirstValue(ClaimTypes.NameIdentifier)!); return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> Confirm(int id, string? version) { await service.ConfirmAsync(id, version); return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> Delete(int id, string? version) { await service.DeleteDraftAsync(id, version); return RedirectToAction(nameof(Index)); }
}
