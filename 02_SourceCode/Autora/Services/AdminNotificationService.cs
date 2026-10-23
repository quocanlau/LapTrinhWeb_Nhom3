using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public class AdminNotificationService(AppDbContext db)
{
                                                                                          
    private async Task<List<AdminNotificationItem>> EventsAsync()
    {
        var since = DateTimeOffset.UtcNow.AddDays(-30);
        var events = new List<AdminNotificationItem>();
        var requests = await db.DepositRequests.AsNoTracking().Where(x => x.CreatedAt >= since)
            .Select(x => new { x.Id, x.CreatedAt, Customer = x.Customer.FullName, Model = x.Car.CarModel.Name, x.Status }).ToListAsync();
        foreach (var x in requests) events.Add(new($"request:{x.Id}", "Yêu cầu đặt cọc", $"{x.Customer} quan tâm {x.Model} · Yêu cầu #{x.Id}",
            (x.Status == RequestStatus.New || x.Status == RequestStatus.Processing) ? $"/Admin/Order?requestId={x.Id}#request-{x.Id}" : "/Admin/Order", x.CreatedAt, "request"));
        var sales = await db.SaleTransactions.AsNoTracking().Where(x => x.ConfirmedAt >= since || (x.Status == SaleStatus.Cancelled && x.UpdatedAt >= since))
            .Select(x => new { x.Id, x.ConfirmedAt, x.UpdatedAt, x.Status, Customer = x.Customer.FullName, Model = x.Car.CarModel.Name }).ToListAsync();
        foreach (var x in sales)
        {
            if (x.ConfirmedAt >= since) events.Add(new($"sale:{x.Id}", "Đã xác nhận giữ xe", $"{x.Model} · {x.Customer} · Giao dịch #{x.Id}", $"/Admin/Order?saleId={x.Id}#sale-{x.Id}", x.ConfirmedAt!.Value, "sale"));
            if (x.Status == SaleStatus.Cancelled && x.UpdatedAt >= since) events.Add(new($"cancel:{x.Id}", "Đã hủy giao dịch", $"{x.Model} · Giao dịch #{x.Id}", $"/Admin/Order?saleId={x.Id}#sale-{x.Id}", x.UpdatedAt, "cancel"));
        }
        var payments = await db.PaymentEntries.AsNoTracking().Where(x => x.CreatedAt >= since)
            .Select(x => new { x.Id, x.CreatedAt, x.Amount, x.EntryType, x.SaleTransactionId, Model = x.SaleTransaction.Car.CarModel.Name }).ToListAsync();
        foreach (var x in payments) events.Add(new($"payment:{x.Id}", x.EntryType == PaymentType.Receipt ? "Đã ghi nhận thu tiền" : "Đã ghi nhận hoàn tiền",
            $"{x.Model} · {x.Amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("vi-VN"))} đ · Giao dịch #{x.SaleTransactionId}", $"/Admin/Order?saleId={x.SaleTransactionId}#sale-{x.SaleTransactionId}", x.CreatedAt, "payment"));
        var deliveries = await db.Deliveries.AsNoTracking().Where(x => x.DeliveredAt >= since)
            .Select(x => new { x.Id, x.DeliveredAt, x.SaleTransactionId, Model = x.SaleTransaction.Car.CarModel.Name }).ToListAsync();
        foreach (var x in deliveries) events.Add(new($"delivery:{x.Id}", "Đã bàn giao xe", $"{x.Model} · Giao dịch #{x.SaleTransactionId}", $"/Admin/Order?saleId={x.SaleTransactionId}#sale-{x.SaleTransactionId}", x.DeliveredAt, "delivery"));
        return events.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Key, StringComparer.Ordinal).ToList();
    }
    public async Task<AdminNotificationPage> ListAsync(string userId, int page = 1, bool unreadOnly = false)
    {
        var events = await EventsAsync();
        var cutoff = DateTimeOffset.UtcNow.AddDays(-31);
        var read = new HashSet<string>(await db.AdminNotificationReads.AsNoTracking().Where(x => x.UserId == userId && x.ReadAt >= cutoff).Select(x => x.EventKey).ToListAsync(), StringComparer.Ordinal);
        var unread = events.Count(x => !read.Contains(x.Key));
        var filtered = events.Where(x => !unreadOnly || !read.Contains(x.Key)).ToList();
        var pages = Math.Max(1, (int)Math.Ceiling(filtered.Count / 20d)); page = Math.Clamp(page, 1, pages);
        return new() { Items = filtered.Skip((page - 1) * 20).Take(20).Select(x => x with { IsRead = read.Contains(x.Key) }).ToList(), UnreadCount = unread, TotalCount = filtered.Count, Page = page, UnreadOnly = unreadOnly };
    }
    public async Task<string?> MarkReadAsync(string userId, string key)
    {
        var item = (await EventsAsync()).FirstOrDefault(x => x.Key == key);
        if (item is null) return null;
        await SaveReadAsync(userId, [key]); return item.Url;
    }
    public async Task MarkAllReadAsync(string userId) => await SaveReadAsync(userId, (await EventsAsync()).Select(x => x.Key).ToArray());
    private async Task SaveReadAsync(string userId, IReadOnlyCollection<string> keys)
    {
                                                                                              
        var cutoff = DateTimeOffset.UtcNow.AddDays(-31);
        for (var attempt = 0; attempt < 3; attempt++)
        {
            var read = new HashSet<string>(await db.AdminNotificationReads.AsNoTracking().Where(x => x.UserId == userId && x.ReadAt >= cutoff).Select(x => x.EventKey).ToListAsync());
            var missing = keys.Distinct().Where(x => !read.Contains(x)).Select(x => new AdminNotificationRead { UserId = userId, EventKey = x }).ToList();
            if (missing.Count == 0) return;
            db.AdminNotificationReads.AddRange(missing);
            try { await db.SaveChangesAsync(); return; }
            catch (DbUpdateException ex) when (attempt < 2 && ex.InnerException is SqlException sql && sql.Number is 2601 or 2627)
            { foreach (var entry in missing) db.Entry(entry).State = EntityState.Detached; }
        }
    }
}
