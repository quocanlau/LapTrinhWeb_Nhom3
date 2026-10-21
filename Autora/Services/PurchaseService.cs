using Autora.Data;
using Autora.Models;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public class PurchaseService(AppDbContext db)
{
    public async Task<int> ImportAsync(int carId, int supplierId, decimal purchasePrice, string employeeId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var car = await db.Cars.SingleOrDefaultAsync(x => x.Id == carId) ?? throw new BusinessException("Không tìm thấy xe.");
        if (car.Status != CarStatus.AwaitingImport || await db.PurchaseDetails.AnyAsync(x => x.CarId == carId))
            throw new BusinessException("Xe này đã nhập hoặc không còn chờ nhập.");
        if (purchasePrice <= 0 || decimal.Round(purchasePrice, 0) != purchasePrice) throw new BusinessException("Giá nhập phải là số nguyên VND lớn hơn 0.");
        if (!await db.Suppliers.AnyAsync(x => x.Id == supplierId && x.IsActive)) throw new BusinessException("Đối tác không hợp lệ.");
        var receipt = new PurchaseReceipt { SupplierId = supplierId, EmployeeId = employeeId, Status = PurchaseStatus.Confirmed };
        receipt.Details.Add(new PurchaseDetail { CarId = carId, PurchasePrice = purchasePrice });
        car.Status = CarStatus.Available; db.PurchaseReceipts.Add(receipt);
        await db.SaveChangesAsync(); await tx.CommitAsync(); return receipt.Id;
    }

    public async Task<int> SaveDraftAsync(Autora.ViewModels.PurchaseDraftForm f, string employeeId)
    {
        if (f.Lines.Count == 0 || f.Lines.Count > 50 || f.Lines.Select(x => x.CarId).Distinct().Count() != f.Lines.Count) throw new BusinessException("Phiếu cần 1–50 xe khác nhau.");
        if (f.Lines.Any(x => x.PurchasePrice <= 0 || decimal.Round(x.PurchasePrice, 0) != x.PurchasePrice)) throw new BusinessException("Giá nhập phải nguyên VND và lớn hơn 0.");
        await using var tx = await db.Database.BeginTransactionAsync();
        if (!await db.Suppliers.AnyAsync(x => x.Id == f.SupplierId && x.IsActive)) throw new BusinessException("Đối tác không hợp lệ.");
        var receipt = f.Id == 0 ? new PurchaseReceipt { EmployeeId = employeeId } : await db.PurchaseReceipts.Include(x => x.Details).SingleOrDefaultAsync(x => x.Id == f.Id);
        if (receipt is null) throw new BusinessException("Không tìm thấy phiếu.");
        if (receipt.Status != PurchaseStatus.Draft) throw new BusinessException("Phiếu đã xác nhận không được sửa.");
        if (f.Id != 0) VersionGuard.Apply(db, receipt, f.Version);
        var ids = f.Lines.Select(x => x.CarId).ToArray();
        if (await db.Cars.CountAsync(x => ids.Contains(x.Id) && x.Status == CarStatus.AwaitingImport) != ids.Length || await db.PurchaseDetails.AnyAsync(x => ids.Contains(x.CarId) && x.PurchaseReceiptId != f.Id)) throw new BusinessException("Xe không còn chờ nhập hoặc thuộc phiếu khác.");
        receipt.SupplierId = f.SupplierId; receipt.PurchasedAt = DateTimeOffset.UtcNow;
        if (f.Id == 0) db.PurchaseReceipts.Add(receipt);
        else { db.PurchaseDetails.RemoveRange(receipt.Details); receipt.Details.Clear(); await db.SaveChangesAsync(); }
        foreach (var line in f.Lines) receipt.Details.Add(new PurchaseDetail { CarId = line.CarId, PurchasePrice = line.PurchasePrice });
        await db.SaveChangesAsync(); await tx.CommitAsync(); return receipt.Id;
    }
    public async Task ConfirmAsync(int id, string? version)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var receipt = await db.PurchaseReceipts.Include(x => x.Supplier).Include(x => x.Details).ThenInclude(x => x.Car).SingleOrDefaultAsync(x => x.Id == id) ?? throw new BusinessException("Không tìm thấy phiếu.");
        if (receipt.Status != PurchaseStatus.Draft || receipt.Details.Count == 0 || !receipt.Supplier.IsActive) throw new BusinessException("Phiếu không đủ điều kiện xác nhận.");
        VersionGuard.Apply(db, receipt, version);
        foreach (var d in receipt.Details) { if (d.Car.Status != CarStatus.AwaitingImport) throw new BusinessException("Có xe không còn chờ nhập."); d.Car.Status = CarStatus.Available; }
        receipt.Status = PurchaseStatus.Confirmed; receipt.PurchasedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(); await tx.CommitAsync();
    }
    public async Task DeleteDraftAsync(int id, string? version)
    {
        var receipt = await db.PurchaseReceipts.Include(x => x.Details).SingleOrDefaultAsync(x => x.Id == id) ?? throw new BusinessException("Không tìm thấy phiếu.");
        if (receipt.Status != PurchaseStatus.Draft) throw new BusinessException("Không xóa phiếu đã xác nhận.");
        VersionGuard.Apply(db, receipt, version); db.PurchaseDetails.RemoveRange(receipt.Details); db.PurchaseReceipts.Remove(receipt); await db.SaveChangesAsync();
    }
}
