using Autora.Data;
using Autora.Models;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public class PurchaseService(AppDbContext db)
{
    public async Task<int> ImportAsync(int carId,int supplierId,decimal purchasePrice,string employeeId)
    {
        await using var tx=await db.Database.BeginTransactionAsync();
        var car=await db.Cars.SingleOrDefaultAsync(x=>x.Id==carId) ?? throw new BusinessException("Không tìm thấy xe.");
        if(car.Status!=CarStatus.AwaitingImport || await db.PurchaseDetails.AnyAsync(x=>x.CarId==carId))
            throw new BusinessException("Xe này đã nhập hoặc không còn chờ nhập.");
        if(purchasePrice<=0 || decimal.Round(purchasePrice,0)!=purchasePrice) throw new BusinessException("Giá nhập phải là số nguyên VND lớn hơn 0.");
        if(!await db.Suppliers.AnyAsync(x=>x.Id==supplierId && x.IsActive)) throw new BusinessException("Đối tác không hợp lệ.");
        var receipt=new PurchaseReceipt{SupplierId=supplierId,EmployeeId=employeeId,Status=PurchaseStatus.Confirmed};
        receipt.Details.Add(new PurchaseDetail{CarId=carId,PurchasePrice=purchasePrice});
        car.Status=CarStatus.Available;db.PurchaseReceipts.Add(receipt);
        await db.SaveChangesAsync();await tx.CommitAsync();return receipt.Id;
    }
}
