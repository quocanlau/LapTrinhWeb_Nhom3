using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public class CarQuery(AppDbContext db)
{
    public async Task<CarCatalog> ListAsync(CarFilter f, bool loadManufacturers = true)
    {
        f.Page = Math.Max(1, f.Page);
        f.PageSize = Math.Clamp(f.PageSize, 1, 50);

        var q = db.Cars.AsNoTracking()
            .Where(x => x.IsPublished && x.Status != CarStatus.AwaitingImport && x.Status != CarStatus.Sold && x.CarModel.IsActive && x.CarModel.Manufacturer.IsActive);

        if (f.ManufacturerId.HasValue) q = q.Where(x => x.CarModel.ManufacturerId == f.ManufacturerId);
        if (!string.IsNullOrWhiteSpace(f.Keyword))
        {
            var k = f.Keyword.Trim();
            q = q.Where(x => x.CarModel.Name.Contains(k) || x.CarModel.Manufacturer.Name.Contains(k));
        }
        if (!string.IsNullOrWhiteSpace(f.Fuel)) q = q.Where(x => x.Fuel == f.Fuel);
        if (f.Condition.HasValue) q = q.Where(x => x.Condition == f.Condition);
        if (f.MaxPrice.HasValue) q = q.Where(x => x.ListedPrice <= f.MaxPrice);

        var total = await q.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(total / (double)f.PageSize));
        f.Page = Math.Min(f.Page, pages);

        var sorted = f.Sort switch
        {
            "price-asc" => q.OrderBy(x => x.ListedPrice).ThenBy(x => x.Id),
            "price-desc" => q.OrderByDescending(x => x.ListedPrice).ThenBy(x => x.Id),
            "year-desc" => q.OrderByDescending(x => x.ManufactureYear).ThenBy(x => x.Id),
            _ => q.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        };

        var cars = await sorted
            .Skip((f.Page - 1) * f.PageSize)
            .Take(f.PageSize)
            .Include(x => x.CarModel).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Images)
            .ToListAsync();

        var manufacturers = loadManufacturers
            ? await db.Manufacturers.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync()
            : [];

        return new CarCatalog
        {
            Filter = f,
            TotalCount = total,
            Cars = cars,
            Manufacturers = manufacturers
        };
    }
}
