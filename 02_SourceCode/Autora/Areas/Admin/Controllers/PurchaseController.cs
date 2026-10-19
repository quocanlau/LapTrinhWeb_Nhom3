using System.Security.Claims;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=Roles.Staff)]
public class PurchaseController(AppDbContext db,PurchaseService service):Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Cars = await db.Cars.AsNoTracking().Include(x => x.CarModel).Where(x => x.Status == CarStatus.AwaitingImport).ToListAsync();
        ViewBag.Suppliers = await db.Suppliers.AsNoTracking().Where(x => x.IsActive).ToListAsync();
        return View(await db.PurchaseReceipts.AsNoTracking().Include(x => x.Supplier).Include(x => x.Details).ThenInclude(x => x.Car).ThenInclude(x => x.CarModel).OrderByDescending(x => x.Id).ToListAsync());
    }
    [HttpPost] public async Task<IActionResult> Import(ImportForm f){if(!ModelState.IsValid)throw new BusinessException("Thông tin nhập xe không hợp lệ.");await service.ImportAsync(f.CarId,f.SupplierId,f.PurchasePrice,User.FindFirstValue(ClaimTypes.NameIdentifier)!);return RedirectToAction("Index");}
    [HttpPost] public async Task<IActionResult> AddSupplier([Bind("Name,SupplierType,Phone,Address")]Supplier supplier){if(!ModelState.IsValid)throw new BusinessException("Thông tin đối tác không hợp lệ.");db.Suppliers.Add(supplier);await db.SaveChangesAsync();return RedirectToAction("Index");}
}
