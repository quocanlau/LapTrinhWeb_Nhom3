using System.Security.Claims;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers.Api;
[ApiController, Route("api/purchase-requests"), Authorize]
public class PurchaseRequestsController(AppDbContext db, SaleService service) : ControllerBase
{
    [HttpPost, Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> Create(RequestForm f)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier); var c = await db.Customers.SingleOrDefaultAsync(x => x.UserId == uid); if (c is null) return Forbid();
        var id = await service.RequestAsync(f.CarId, c.Id, f.Note); return CreatedAtAction(nameof(Get), new { id }, new { id });
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier); var staff = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Employee);
        var item = await db.DepositRequests.AsNoTracking().Where(x => x.Id == id && (staff || x.Customer.UserId == uid)).Select(x => new { x.Id, x.CarId, x.Note, status = x.Status.ToString(), x.CreatedAt }).SingleOrDefaultAsync();
        return item is null ? NotFound() : Ok(item);
    }
}
