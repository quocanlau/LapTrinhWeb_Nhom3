using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class CarImageController(AppDbContext db, IWebHostEnvironment env) : Controller
{
    private async Task<Car> Load(int id)
    {
        var c = await db.Cars.Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id) ?? throw new BusinessException("Không tìm thấy xe.");
        if (c.Status is CarStatus.Reserved or CarStatus.Deposited or CarStatus.Sold) throw new BusinessException("Không sửa ảnh xe đang giữ, đã cọc hoặc đã bán.");
        db.Entry(c).Property(x => x.IsPublished).IsModified = true; return c;
    }
    public async Task<IActionResult> Index(int id) { var c = await db.Cars.AsNoTracking().Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id); return c is null ? NotFound() : View(c); }
    [HttpPost, RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Add(int carId, IFormFile? file, string? imageUrl, int sortOrder = 0)
    {
        var c = await Load(carId); if (c.Images.Count >= 12) throw new BusinessException("Mỗi xe tối đa 12 ảnh.");
        string? path = null; string? saved = null;
        if (file is not null && file.Length > 0)
        {
            if (file.Length > 5 * 1024 * 1024) throw new BusinessException("Ảnh tối đa 5 MB.");
            await using var input = file.OpenReadStream(); var header = new byte[12]; var n = await input.ReadAsync(header); var ext = ImageUpload.DetectExtension(header.AsSpan(0, n));
            var given = Path.GetExtension(file.FileName).ToLowerInvariant(); if (ext is null || (given != ext && !(ext == ".jpg" && given == ".jpeg"))) throw new BusinessException("Chỉ chấp nhận ảnh PNG, JPEG hoặc WebP đúng nội dung và đuôi file.");
            var name = Guid.NewGuid().ToString("N") + ext; var directory = Path.Combine(env.WebRootPath, "images", "cars"); Directory.CreateDirectory(directory); saved = Path.Combine(directory, name);
            input.Position = 0; await using (var output = new FileStream(saved, FileMode.CreateNew)) { await input.CopyToAsync(output); }
            path = "/images/cars/" + name;
        }
        else { if (!ImageUpload.IsAllowedUrl(imageUrl)) throw new BusinessException("Nhập URL HTTPS hoặc đường dẫn ảnh /images/cars/ hợp lệ."); path = imageUrl; }
        try { c.Images.Add(new CarImage { ImagePath = path!, IsPrimary = c.Images.Count == 0, SortOrder = Math.Clamp(sortOrder, 0, 1000) }); await db.SaveChangesAsync(); }
        catch { if (saved is not null) System.IO.File.Delete(saved); throw; }
        return RedirectToAction(nameof(Index), new { id = carId });
    }
    [HttpPost]
    public async Task<IActionResult> Primary(int carId, int imageId)
    {
        await using var tx = await db.Database.BeginTransactionAsync(); var c = await Load(carId); var image = c.Images.SingleOrDefault(x => x.Id == imageId) ?? throw new BusinessException("Ảnh không thuộc xe.");
        foreach (var x in c.Images) x.IsPrimary = false; await db.SaveChangesAsync(); image.IsPrimary = true; await db.SaveChangesAsync(); await tx.CommitAsync(); return RedirectToAction(nameof(Index), new { id = carId });
    }
    [HttpPost]
    public async Task<IActionResult> Update(int carId, int imageId, int sortOrder)
    {
        var c = await Load(carId); var image = c.Images.SingleOrDefault(x => x.Id == imageId) ?? throw new BusinessException("Ảnh không thuộc xe."); image.SortOrder = Math.Clamp(sortOrder, 0, 1000); await db.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { id = carId });
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int carId, int imageId)
    {
        await using var tx = await db.Database.BeginTransactionAsync(); var c = await Load(carId); var image = c.Images.SingleOrDefault(x => x.Id == imageId) ?? throw new BusinessException("Ảnh không thuộc xe.");
        db.CarImages.Remove(image); c.Images.Remove(image); await db.SaveChangesAsync(); if (image.IsPrimary && c.Images.Count > 0) { c.Images.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).First().IsPrimary = true; await db.SaveChangesAsync(); }
        await tx.CommitAsync(); return RedirectToAction(nameof(Index), new { id = carId });
    }
}
