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
public class OrderController(AppDbContext db, SaleService service) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int? saleId = null, int? requestId = null)
    {
        var requests = db.DepositRequests.AsNoTracking().Where(x => x.Status == RequestStatus.New || x.Status == RequestStatus.Processing); if (requestId.HasValue) requests = requests.Where(x => x.Id == requestId);
        var sales = db.SaleTransactions.AsNoTracking(); if (saleId.HasValue) sales = sales.Where(x => x.Id == saleId);
        var total = Math.Max(await requests.CountAsync(), await sales.CountAsync()); var pages = Math.Max(1, (int)Math.Ceiling(total / 20d)); page = Math.Clamp(page, 1, pages);
        return View(new StaffOrders { Page = page, Pages = pages, Requests = await requests.OrderByDescending(x => x.Id).Skip((page - 1) * 20).Take(20).Include(x => x.Customer).Include(x => x.Car).ThenInclude(x => x.CarModel).ToListAsync(), Sales = await sales.OrderByDescending(x => x.Id).Skip((page - 1) * 20).Take(20).Include(x => x.Customer).Include(x => x.Car).ThenInclude(x => x.CarModel).Include(x => x.Payments).ToListAsync() });
    }
    [HttpPost] public async Task<IActionResult> Reserve(ReserveForm form) { if (!ModelState.IsValid) throw new BusinessException("Giá chốt không hợp lệ."); await service.ReserveAsync(form.RequestId, form.AgreedPrice, User.FindFirstValue(ClaimTypes.NameIdentifier)!); return RedirectToAction("Index"); }
    [HttpPost] public async Task<IActionResult> Pay(PayForm form) { if (!ModelState.IsValid) throw new BusinessException("Dữ liệu thu hoặc hoàn tiền không hợp lệ."); await service.PayAsync(form.SaleId, form.EntryType, form.Amount, form.PaymentMethod, form.Note, User.FindFirstValue(ClaimTypes.NameIdentifier)!); return RedirectToAction("Index"); }
    [HttpPost] public async Task<IActionResult> Deliver(DeliveryForm form) { if (!ModelState.IsValid) throw new BusinessException("Thông tin bàn giao không hợp lệ."); await service.DeliverAsync(form.SaleId, form.Recipient, form.Note); return RedirectToAction("Index"); }
    [HttpPost] public async Task<IActionResult> Cancel(int saleId) { await service.CancelAsync(saleId); return RedirectToAction("Index"); }
    [HttpPost] public async Task<IActionResult> CloseRequest(int requestId) { var r = await db.DepositRequests.SingleOrDefaultAsync(x => x.Id == requestId); if (r is null) return NotFound(); if (r.Status is not (RequestStatus.New or RequestStatus.Processing)) throw new BusinessException("Yêu cầu đã xử lý."); r.Status = RequestStatus.Closed; await db.SaveChangesAsync(); return RedirectToAction("Index"); }

    [HttpPost] public async Task<IActionResult> Process(int requestId) { var r = await db.DepositRequests.FindAsync(requestId); if (r is null) return NotFound(); if (r.Status != RequestStatus.New) throw new BusinessException("Yêu cầu đã được xử lý."); r.Status = RequestStatus.Processing; await db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    private async Task Choices() { ViewBag.Customers = await db.Customers.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(); ViewBag.Cars = await db.Cars.AsNoTracking().Where(CarVisibility.Public).Where(x => x.Status == CarStatus.Available).Include(x => x.CarModel).ToListAsync(); }
    public async Task<IActionResult> Draft(int id = 0) { await Choices(); if (id == 0) return View(new SaleDraftForm()); var s = await db.SaleTransactions.FindAsync(id); if (s is null) return NotFound(); if (s.Status != SaleStatus.Draft) throw new BusinessException("Không sửa giao dịch đã xác nhận."); return View(new SaleDraftForm { Id = s.Id, CarId = s.CarId, CustomerId = s.CustomerId, AgreedPrice = s.AgreedPrice, Version = Convert.ToBase64String(s.RowVersion) }); }
    [HttpPost] public async Task<IActionResult> SaveDraft(SaleDraftForm f) { if (!ModelState.IsValid) { await Choices(); return View("Draft", f); } await service.SaveDraftAsync(f, User.FindFirstValue(ClaimTypes.NameIdentifier)!); return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> ConfirmDraft(int id, string? version) { await service.ConfirmDraftAsync(id, version); return RedirectToAction(nameof(Index)); }
}
