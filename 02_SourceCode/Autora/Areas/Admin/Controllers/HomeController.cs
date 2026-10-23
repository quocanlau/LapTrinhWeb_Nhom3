using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(DateOnly? from = null, DateOnly? to = null)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)); var start = from ?? new DateOnly(today.Year, today.Month, 1); var end = to ?? today;
        if (end < start || end.DayNumber - start.DayNumber > 366) return BadRequest("Khoảng ngày phải từ 0 đến 366 ngày.");
        var a = new DateTimeOffset(start.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7)); var b = new DateTimeOffset(end.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7));
        var delivered = db.Deliveries.AsNoTracking().Where(x => x.DeliveredAt >= a && x.DeliveredAt < b);
        var gross = from d in delivered join p in db.PurchaseDetails on d.SaleTransaction.CarId equals p.CarId select d.SaleTransaction.AgreedPrice - p.PurchasePrice;
        var balances = await db.SaleTransactions.AsNoTracking().Where(x => (x.Status == SaleStatus.Reserved || x.Status == SaleStatus.Deposited) && x.ConfirmedAt >= a && x.ConfirmedAt < b).Select(x => new { x.AgreedPrice, Paid = x.Payments.Sum(p => p.EntryType == PaymentType.Receipt ? p.Amount : -p.Amount) }).ToListAsync();
        return View(new DashboardModel { From = start, To = end, Stock = await db.Cars.CountAsync(x => x.Status == CarStatus.Available || x.Status == CarStatus.Reserved || x.Status == CarStatus.Deposited), Sold = await delivered.CountAsync(), Revenue = await delivered.SumAsync(x => (decimal?)x.SaleTransaction.AgreedPrice) ?? 0, NetCash = await db.PaymentEntries.Where(x => x.CreatedAt >= a && x.CreatedAt < b).SumAsync(x => (decimal?)(x.EntryType == PaymentType.Receipt ? x.Amount : -x.Amount)) ?? 0, PurchaseValue = await db.PurchaseDetails.Where(x => x.PurchaseReceipt.Status == PurchaseStatus.Confirmed && x.PurchaseReceipt.PurchasedAt >= a && x.PurchaseReceipt.PurchasedAt < b).SumAsync(x => (decimal?)x.PurchasePrice) ?? 0, GrossProfit = await gross.SumAsync(x => (decimal?)x) ?? 0, Receivable = balances.Sum(x => x.AgreedPrice - x.Paid) });
    }
}
