using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class CustomerController(AppDbContext db, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? keyword = null)
    {
        var q = db.Customers.AsNoTracking(); if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(x => x.FullName.Contains(keyword) || x.Phone.Contains(keyword));
        return View(await q.OrderBy(x => x.FullName).ThenBy(x => x.Id).PageAsync(page, keyword));
    }
    public async Task<IActionResult> Edit(int id = 0)
    {
        if (id == 0) return View(new CustomerForm()); var c = await db.Customers.FindAsync(id); return c is null ? NotFound() : View(new CustomerForm { Id = c.Id, FullName = c.FullName, Phone = c.Phone, Email = c.Email, Address = c.Address });
    }
    [HttpPost]
    public async Task<IActionResult> Save(CustomerForm f)
    {
        if (!ModelState.IsValid) return View("Edit", f);
        var c = f.Id == 0 ? new Customer() : await db.Customers.FindAsync(f.Id); if (c is null) return NotFound();
        await using var tx = await db.Database.BeginTransactionAsync();
        if (c.UserId is not null)
        {
            var u = await users.FindByIdAsync(c.UserId); if (u is null) throw new BusinessException("Không tìm thấy tài khoản liên kết.");
            if (!string.Equals(f.Email, u.Email, StringComparison.OrdinalIgnoreCase)) { ModelState.AddModelError("Email", "Không đổi email đăng nhập tại hồ sơ khách liên kết."); return View("Edit", f); }
            u.FullName = f.FullName.Trim(); u.PhoneNumber = f.Phone.Trim(); var result = await users.UpdateAsync(u);
            if (!result.Succeeded) throw new BusinessException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
        c.FullName = f.FullName.Trim(); c.Phone = f.Phone.Trim(); c.Email = f.Email; c.Address = f.Address; if (f.Id == 0) db.Customers.Add(c);
        await db.SaveChangesAsync(); await tx.CommitAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Customers.FindAsync(id); if (c is null) return NotFound();
        if (c.UserId != null || await db.DepositRequests.AnyAsync(x => x.CustomerId == id) || await db.SaleTransactions.AnyAsync(x => x.CustomerId == id)) throw new BusinessException("Không xóa khách có tài khoản hoặc chứng từ. Hãy giữ hồ sơ lịch sử.");
        db.Customers.Remove(c); await db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
}
