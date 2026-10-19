using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class CarController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? keyword = null) { var q = db.Cars.AsNoTracking().Include(x => x.CarModel).ThenInclude(x => x.Manufacturer).AsQueryable(); if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(x => x.VIN.Contains(keyword) || x.CarModel.Name.Contains(keyword)); var data = await q.OrderByDescending(x => x.Id).PageAsync(page, keyword); ViewBag.Page = data.Page; ViewBag.Pages = data.Pages; ViewBag.Keyword = keyword; return View(data.Items); }
    private async Task Choices() => ViewBag.CarModels = await db.CarModels.AsNoTracking().Include(x => x.Manufacturer).Where(x => x.IsActive && x.Manufacturer.IsActive).OrderBy(x => x.Name).ToListAsync();
    public async Task<IActionResult> Create() { await Choices(); return View("Edit", new CarForm()); }
    public async Task<IActionResult> Edit(int id) { var c = await db.Cars.Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id); if (c is null) return NotFound(); await Choices(); return View(new CarForm { Id = c.Id, CarModelId = c.CarModelId, VIN = c.VIN, ManufactureYear = c.ManufactureYear, Color = c.Color, Condition = c.Condition, Mileage = c.Mileage, Fuel = c.Fuel, Transmission = c.Transmission, ListedPrice = c.ListedPrice, IsPublished = c.IsPublished, ImageUrl = c.Images.FirstOrDefault(x => x.IsPrimary)?.ImagePath, Version = Convert.ToBase64String(c.RowVersion) }); }
    [HttpPost]
    public async Task<IActionResult> Save(CarForm f)
    {
        f.VIN = f.VIN?.Trim().ToUpperInvariant() ?? "";
        ModelState.Remove(nameof(f.VIN)); TryValidateModel(f);
        if (f.Condition == CarCondition.New && f.Mileage > 100) ModelState.AddModelError(nameof(f.Mileage), "Xe mới quy ước không quá 100 km.");
        if (f.ManufactureYear > DateTime.UtcNow.Year + 1) ModelState.AddModelError(nameof(f.ManufactureYear), "Năm sản xuất vượt giới hạn.");
        if (f.ListedPrice > 0 && decimal.Round(f.ListedPrice, 0) != f.ListedPrice) ModelState.AddModelError(nameof(f.ListedPrice), "Giá phải là số nguyên VND.");
        if (!await db.CarModels.AnyAsync(x => x.Id == f.CarModelId && x.IsActive && x.Manufacturer.IsActive)) ModelState.AddModelError(nameof(f.CarModelId), "Dòng xe không hợp lệ.");
        f.ImageUrl = f.ImageUrl?.Trim();
        if (!string.IsNullOrEmpty(f.ImageUrl) && !ImageUpload.IsAllowedUrl(f.ImageUrl))
            ModelState.AddModelError(nameof(f.ImageUrl), "Ảnh dùng URL HTTPS hoặc đường dẫn /images/cars/ hợp lệ.");
        if(f.ListedPrice>0 && f.ListedPrice<25)ModelState.AddModelError(nameof(f.ListedPrice),"Giá quá nhỏ để tính cọc 2% làm tròn nguyên đồng.");
        if (!ModelState.IsValid) { await Choices(); return View("Edit", f); }
        await using var tx = await db.Database.BeginTransactionAsync();
        var c = f.Id == 0 ? new Car() : await db.Cars.Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == f.Id);
        if (c is null) return NotFound();
        if (c.Status is CarStatus.Reserved or CarStatus.Deposited or CarStatus.Sold) throw new BusinessException("Không sửa hồ sơ xe đang giữ, đã cọc hoặc đã bán.");
        if (f.Id != 0) { if (string.IsNullOrEmpty(f.Version)) throw new BusinessException("Thiếu phiên bản dữ liệu."); try { db.Entry(c).Property(x => x.RowVersion).OriginalValue = Convert.FromBase64String(f.Version); } catch (FormatException) { throw new BusinessException("Phiên bản dữ liệu không hợp lệ."); } }
        c.CarModelId = f.CarModelId; c.VIN = f.VIN; c.ManufactureYear = f.ManufactureYear; c.Color = f.Color; c.Condition = f.Condition; c.Mileage = f.Mileage; c.Fuel = f.Fuel; c.Transmission = f.Transmission; c.ListedPrice = f.ListedPrice; c.IsPublished = f.IsPublished;
        if (f.Id == 0) db.Cars.Add(c);
        var image = c.Images.FirstOrDefault(x => x.IsPrimary);
        if (string.IsNullOrWhiteSpace(f.ImageUrl)) { if (image is not null) { db.CarImages.Remove(image); c.Images.Remove(image); await db.SaveChangesAsync(); var next = c.Images.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).FirstOrDefault(); if (next is not null) next.IsPrimary = true; } }
        else if (image is not null) image.ImagePath = f.ImageUrl; else c.Images.Add(new CarImage { ImagePath = f.ImageUrl, IsPrimary = true });
        await db.SaveChangesAsync(); await tx.CommitAsync(); return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Cars.Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id); if (c is null) return NotFound();
        if (c.Status != CarStatus.AwaitingImport || await db.PurchaseDetails.AnyAsync(x => x.CarId == id) || await db.DepositRequests.AnyAsync(x => x.CarId == id) || await db.SaleTransactions.AnyAsync(x => x.CarId == id)) throw new BusinessException("Chỉ xóa xe chờ nhập chưa có chứng từ hoặc yêu cầu.");
        db.CarImages.RemoveRange(c.Images); db.Cars.Remove(c); await db.SaveChangesAsync(); return RedirectToAction("Index");
    }
}
