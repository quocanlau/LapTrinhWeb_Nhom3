using System.Security.Claims;
using Autora.Data;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers;
[Authorize(Roles = Autora.Models.Roles.Customer)]
public class DepositController(AppDbContext db, SaleService sales) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Create(RequestForm form)
    {
        if (!ModelState.IsValid) throw new BusinessException("Yêu cầu không hợp lệ.");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var customer = await db.Customers.SingleAsync(x => x.UserId == userId);
        await sales.RequestAsync(form.CarId, customer.Id, form.Note);
        TempData["Message"] = "Đã lưu yêu cầu. Đây chưa phải xác nhận nhận tiền hoặc giữ xe.";
        return RedirectToAction("Mine");
    }
    public async Task<IActionResult> Mine()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return View(new CustomerOrders
        {
            Requests = await db.DepositRequests.AsNoTracking().Include(x => x.Car).ThenInclude(x => x.CarModel).Where(x => x.Customer.UserId == id).OrderByDescending(x => x.Id).ToListAsync(),
            Sales = await db.SaleTransactions.AsNoTracking().Include(x => x.Car).ThenInclude(x => x.CarModel).Include(x => x.Payments).Where(x => x.Customer.UserId == id).OrderByDescending(x => x.Id).ToListAsync()
        });
    }
}
