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
public class OrderController(AppDbContext db,SaleService service):Controller
{
    public async Task<IActionResult> Index()=>View(new StaffOrders{
        Requests=await db.DepositRequests.AsNoTracking().Include(x=>x.Customer).Include(x=>x.Car).ThenInclude(x=>x.CarModel).Where(x=>x.Status==RequestStatus.New).OrderByDescending(x=>x.Id).ToListAsync(),
        Sales=await db.SaleTransactions.AsNoTracking().Include(x=>x.Customer).Include(x=>x.Car).ThenInclude(x=>x.CarModel).Include(x=>x.Payments).OrderByDescending(x=>x.Id).ToListAsync()});
    [HttpPost] public async Task<IActionResult> Reserve(ReserveForm form){if(!ModelState.IsValid)throw new BusinessException("Giá chốt không hợp lệ.");await service.ReserveAsync(form.RequestId,form.AgreedPrice,User.FindFirstValue(ClaimTypes.NameIdentifier)!);return RedirectToAction("Index");}
    [HttpPost] public async Task<IActionResult> Pay(PayForm form){if(!ModelState.IsValid)throw new BusinessException("Dữ liệu thu hoặc hoàn tiền không hợp lệ.");await service.PayAsync(form.SaleId,form.EntryType,form.Amount,form.PaymentMethod,form.Note,User.FindFirstValue(ClaimTypes.NameIdentifier)!);return RedirectToAction("Index");}
    [HttpPost] public async Task<IActionResult> Deliver(DeliveryForm form){if(!ModelState.IsValid)throw new BusinessException("Thông tin bàn giao không hợp lệ.");await service.DeliverAsync(form.SaleId,form.Recipient,form.Note);return RedirectToAction("Index");}
    [HttpPost] public async Task<IActionResult> Cancel(int saleId){await service.CancelAsync(saleId);return RedirectToAction("Index");}
    [HttpPost] public async Task<IActionResult> CloseRequest(int requestId){var r=await db.DepositRequests.SingleOrDefaultAsync(x=>x.Id==requestId);if(r is null)return NotFound();if(r.Status!=RequestStatus.New)throw new BusinessException("Yêu cầu đã xử lý.");r.Status=RequestStatus.Closed;await db.SaveChangesAsync();return RedirectToAction("Index");}
}
