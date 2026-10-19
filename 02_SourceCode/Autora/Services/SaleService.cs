using System.Data;
using Autora.Data;
using Autora.Models;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public class SaleService(AppDbContext db)
{
    public async Task<int> RequestAsync(int carId, int customerId, string? note)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var car = await db.Cars.Include(x => x.CarModel).ThenInclude(x => x.Manufacturer).SingleOrDefaultAsync(x => x.Id == carId) ?? throw new BusinessException("Không tìm thấy xe.");
        DepositPolicy.RequireAvailable(car);
        if (!car.CarModel.IsActive || !car.CarModel.Manufacturer.IsActive) throw new BusinessException("Xe đã ngừng mở bán.");
        if (await db.DepositRequests.AnyAsync(x => x.CarId == carId && x.CustomerId == customerId && (x.Status == RequestStatus.New || x.Status == RequestStatus.Processing)))
            throw new BusinessException("Bạn đã có yêu cầu đang chờ cho xe này.");
        var request = new DepositRequest { CarId = carId, CustomerId = customerId, Note = note };
        db.DepositRequests.Add(request); await db.SaveChangesAsync(); await tx.CommitAsync(); return request.Id;
    }
    public async Task<int> ReserveAsync(int requestId, decimal agreedPrice, string employeeId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var request = await db.DepositRequests.Include(x => x.Car).ThenInclude(x => x.CarModel).ThenInclude(x => x.Manufacturer).SingleOrDefaultAsync(x => x.Id == requestId)
            ?? throw new BusinessException("Không tìm thấy yêu cầu.");
        if (request.Status is not (RequestStatus.New or RequestStatus.Processing)) throw new BusinessException("Yêu cầu đã được xử lý.");
        DepositPolicy.RequireAvailable(request.Car);
        if (!request.Car.CarModel.IsActive || !request.Car.CarModel.Manufacturer.IsActive) throw new BusinessException("Xe đã ngừng mở bán.");
        if (decimal.Round(agreedPrice, 0) != agreedPrice) throw new BusinessException("Giá bán phải là số nguyên VND.");
        var sale = new SaleTransaction
        {
            CarId = request.CarId,
            CustomerId = request.CustomerId,
            DepositRequestId = request.Id,
            EmployeeId = employeeId,
            AgreedPrice = agreedPrice,
            RequiredDeposit = DepositPolicy.Calculate(agreedPrice),
            Status = SaleStatus.Reserved,
            ConfirmedAt = DateTimeOffset.UtcNow
        };
        request.Status = RequestStatus.Converted; request.Car.Status = CarStatus.Reserved;
        db.SaleTransactions.Add(sale); await db.SaveChangesAsync(); await tx.CommitAsync(); return sale.Id;
    }
    public async Task PayAsync(int saleId, PaymentType type, decimal amount, string method, string? note, string employeeId)
    {
        if (!Enum.IsDefined(type)) throw new BusinessException("Loại thu hoặc hoàn không hợp lệ.");
        await using var tx = await db.Database.BeginTransactionAsync();
        var sale = await db.SaleTransactions.Include(x => x.Car).Include(x => x.Payments).SingleOrDefaultAsync(x => x.Id == saleId)
            ?? throw new BusinessException("Không tìm thấy giao dịch.");
        if (sale.Car.Status is not (CarStatus.Reserved or CarStatus.Deposited)) throw new BusinessException("Trạng thái xe không hợp lệ.");
        DepositPolicy.ValidatePayment(sale, type, amount);
        var net = DepositPolicy.NetPaid(sale) + (type == PaymentType.Receipt ? amount : -amount);
        sale.Payments.Add(new PaymentEntry { EntryType = type, Amount = amount, PaymentMethod = method, Note = note, CreatedById = employeeId });
        sale.Status = net >= sale.RequiredDeposit ? SaleStatus.Deposited : SaleStatus.Reserved;
        sale.Car.Status = net >= sale.RequiredDeposit ? CarStatus.Deposited : CarStatus.Reserved;
        sale.UpdatedAt = DateTimeOffset.UtcNow;
        db.Entry(sale).Property(x => x.UpdatedAt).IsModified = true;
        await db.SaveChangesAsync(); await tx.CommitAsync();
    }
    public async Task DeliverAsync(int saleId, string recipient, string? note)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var sale = await db.SaleTransactions.Include(x => x.Car).Include(x => x.Payments).SingleOrDefaultAsync(x => x.Id == saleId)
            ?? throw new BusinessException("Không tìm thấy giao dịch.");
        if (sale.Status != SaleStatus.Deposited || sale.Car.Status != CarStatus.Deposited || DepositPolicy.NetPaid(sale) != sale.AgreedPrice)
            throw new BusinessException("Chỉ bàn giao khi giao dịch đã cọc và thanh toán đủ.");
        sale.Status = SaleStatus.Delivered; sale.Car.Status = CarStatus.Sold; sale.UpdatedAt = DateTimeOffset.UtcNow;
        db.Deliveries.Add(new Delivery { SaleTransactionId = sale.Id, Recipient = recipient, Note = note });
        await db.SaveChangesAsync(); await tx.CommitAsync();
    }
    public async Task CancelAsync(int saleId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var sale = await db.SaleTransactions.Include(x => x.Car).Include(x => x.Payments).SingleOrDefaultAsync(x => x.Id == saleId)
            ?? throw new BusinessException("Không tìm thấy giao dịch.");
        if (sale.Status is not (SaleStatus.Draft or SaleStatus.Reserved or SaleStatus.Deposited)) throw new BusinessException("Giao dịch không thể hủy.");
        if (DepositPolicy.NetPaid(sale) != 0) throw new BusinessException("Cần hoàn đủ khoản tiền đã thu trước khi hủy. Bản đầu chưa hỗ trợ giữ lại cọc.");
        if (sale.Status != SaleStatus.Draft) sale.Car.Status = CarStatus.Available;
        sale.Status = SaleStatus.Cancelled; sale.UpdatedAt = DateTimeOffset.UtcNow;
        db.Entry(sale).Property(x => x.UpdatedAt).IsModified = true;
        await db.SaveChangesAsync(); await tx.CommitAsync();
    }

    public async Task<int> SaveDraftAsync(Autora.ViewModels.SaleDraftForm f, string employeeId)
    {
        if (f.AgreedPrice <= 0 || decimal.Round(f.AgreedPrice, 0) != f.AgreedPrice) throw new BusinessException("Giá bán phải nguyên VND và lớn hơn 0.");
        await using var tx = await db.Database.BeginTransactionAsync();
        if (!await db.Customers.AnyAsync(x => x.Id == f.CustomerId)) throw new BusinessException("Không tìm thấy khách hàng.");
        var car = await db.Cars.Include(x => x.CarModel).ThenInclude(x => x.Manufacturer).SingleOrDefaultAsync(x => x.Id == f.CarId) ?? throw new BusinessException("Không tìm thấy xe.");
        DepositPolicy.RequireAvailable(car); if (!car.CarModel.IsActive || !car.CarModel.Manufacturer.IsActive) throw new BusinessException("Xe đã ngừng mở bán.");
        var sale = f.Id == 0 ? new SaleTransaction { EmployeeId = employeeId } : await db.SaleTransactions.FindAsync(f.Id);
        if (sale is null || sale.Status != SaleStatus.Draft) throw new BusinessException("Chỉ sửa giao dịch nháp.");
        if (f.Id != 0) VersionGuard.Apply(db, sale, f.Version);
        sale.CustomerId = f.CustomerId; sale.CarId = f.CarId; sale.AgreedPrice = f.AgreedPrice; sale.RequiredDeposit = DepositPolicy.Calculate(f.AgreedPrice); sale.UpdatedAt = DateTimeOffset.UtcNow;
        if (f.Id == 0) db.SaleTransactions.Add(sale); await db.SaveChangesAsync(); await tx.CommitAsync(); return sale.Id;
    }
    public async Task ConfirmDraftAsync(int id, string? version)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var sale = await db.SaleTransactions.Include(x => x.Car).ThenInclude(x => x.CarModel).ThenInclude(x => x.Manufacturer).SingleOrDefaultAsync(x => x.Id == id) ?? throw new BusinessException("Không tìm thấy giao dịch.");
        if (sale.Status != SaleStatus.Draft) throw new BusinessException("Giao dịch không còn là nháp.");
        VersionGuard.Apply(db, sale, version); DepositPolicy.RequireAvailable(sale.Car);
        if (!sale.Car.CarModel.IsActive || !sale.Car.CarModel.Manufacturer.IsActive) throw new BusinessException("Xe đã ngừng mở bán.");
        sale.Status = SaleStatus.Reserved; sale.ConfirmedAt = DateTimeOffset.UtcNow; sale.UpdatedAt = DateTimeOffset.UtcNow; sale.Car.Status = CarStatus.Reserved;
        await db.SaveChangesAsync(); await tx.CommitAsync();
    }
}
