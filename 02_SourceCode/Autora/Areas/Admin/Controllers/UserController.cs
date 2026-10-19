using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Admin)]
public class UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roles, AppDbContext db) : Controller
{
    private void Require(IdentityResult r) { if (!r.Succeeded) throw new BusinessException(string.Join("; ", r.Errors.Select(x => x.Description))); }
    public async Task<IActionResult> Index(int page = 1, string? keyword = null)
    {
        var q = db.Users.AsNoTracking(); if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(x => x.FullName.Contains(keyword) || x.Email!.Contains(keyword));
        var data = await q.OrderBy(x => x.FullName).ThenBy(x => x.Id).PageAsync(page, keyword);
        var ids = data.Items.Select(x => x.Id).ToArray(); var map = await (from ur in db.UserRoles join role in db.Roles on ur.RoleId equals role.Id where ids.Contains(ur.UserId) select new { ur.UserId, role.Name }).ToListAsync();
        var current = userManager.GetUserId(User);
        return View(new UserListViewModel { Page = data.Page, Pages = data.Pages, Total = data.Total, Keyword = keyword, Users = data.Items.Select(u => new UserItemViewModel { Id = u.Id, FullName = u.FullName, Email = u.Email ?? "", Phone = u.PhoneNumber ?? "", IsActive = u.IsActive, IsCurrentAdmin = u.Id == current, Role = map.Where(x => x.UserId == u.Id).Select(x => x.Name!).OrderBy(x => x == Roles.Admin ? 0 : x == Roles.Employee ? 1 : 2).FirstOrDefault() ?? "" }).ToList() });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateForm f)
    {
        if (!ModelState.IsValid) { TempData["Error"] = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)); return RedirectToAction(nameof(Index)); }
        if (f.Role is not (Roles.Admin or Roles.Employee)) throw new BusinessException("Vai trò không hợp lệ.");
        await using var tx = await db.Database.BeginTransactionAsync();
        if (!await roles.RoleExistsAsync(f.Role)) Require(await roles.CreateAsync(new IdentityRole(f.Role)));
        var u = new ApplicationUser { UserName = f.Email.Trim(), Email = f.Email.Trim(), FullName = f.FullName.Trim(), PhoneNumber = f.Phone, IsActive = true };
        Require(await userManager.CreateAsync(u, f.Password)); Require(await userManager.AddToRoleAsync(u, f.Role)); await tx.CommitAsync(); TempData["Success"] = "Đã tạo tài khoản."; return RedirectToAction(nameof(Index));
    }
    private async Task GuardAdmin(ApplicationUser u, bool removesAdmin)
    {
        if (!removesAdmin || !await userManager.IsInRoleAsync(u, Roles.Admin)) return;
        if (u.Id == userManager.GetUserId(User)) throw new BusinessException("Không tự khóa, xóa hoặc hạ quyền tài khoản đang đăng nhập.");
        var admins = await userManager.GetUsersInRoleAsync(Roles.Admin); if (u.IsActive && admins.Count(x => x.IsActive) <= 1) throw new BusinessException("Phải giữ ít nhất một quản trị viên hoạt động.");
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var u = await userManager.FindByIdAsync(id); if (u is null) return NotFound(); await GuardAdmin(u, u.IsActive);
        if (u.Id == userManager.GetUserId(User)) throw new BusinessException("Không tự khóa tài khoản.");
        u.IsActive = !u.IsActive; Require(await userManager.UpdateAsync(u)); Require(await userManager.UpdateSecurityStampAsync(u)); await tx.CommitAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(string id, string role)
    {
        if (role is not (Roles.Admin or Roles.Employee or Roles.Customer)) return BadRequest();
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var u = await userManager.FindByIdAsync(id); if (u is null) return NotFound(); await GuardAdmin(u, role != Roles.Admin);
        if (!await roles.RoleExistsAsync(role)) Require(await roles.CreateAsync(new IdentityRole(role)));
        var old = await userManager.GetRolesAsync(u); Require(await userManager.RemoveFromRolesAsync(u, old)); Require(await userManager.AddToRoleAsync(u, role));
        if (role == Roles.Customer && !await db.Customers.AnyAsync(x => x.UserId == u.Id)) db.Customers.Add(new Customer { UserId = u.Id, FullName = u.FullName, Phone = u.PhoneNumber ?? "Chưa cập nhật", Email = u.Email });
        Require(await userManager.UpdateSecurityStampAsync(u)); await db.SaveChangesAsync(); await tx.CommitAsync(); return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Reset(string id) { var u = await userManager.FindByIdAsync(id); if (u is null) return NotFound(); ViewBag.UserId = id; ViewBag.UserName = u.FullName; return View(); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8 || newPassword.Length > 100) throw new BusinessException("Nhập mật khẩu mới từ 8 đến 100 ký tự, đủ hoa/thường/số/ký tự đặc biệt.");
        var u = await userManager.FindByIdAsync(id); if (u is null) return NotFound(); var token = await userManager.GeneratePasswordResetTokenAsync(u); Require(await userManager.ResetPasswordAsync(u, token, newPassword)); TempData["Success"] = "Đã đặt mật khẩu mới. Phiên đăng nhập cũ bị vô hiệu hóa."; return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var u = await userManager.FindByIdAsync(id); if (u is null) return NotFound(); await GuardAdmin(u, true);
        if (u.Id == userManager.GetUserId(User)) throw new BusinessException("Không tự xóa tài khoản.");
        var history = await db.SaleTransactions.AnyAsync(x => x.EmployeeId == id) || await db.PurchaseReceipts.AnyAsync(x => x.EmployeeId == id) || await db.PaymentEntries.AnyAsync(x => x.CreatedById == id) || await db.Customers.AnyAsync(c => c.UserId == id && (db.SaleTransactions.Any(x => x.CustomerId == c.Id) || db.DepositRequests.Any(x => x.CustomerId == c.Id)));
        if (history) { u.IsActive = false; Require(await userManager.UpdateAsync(u)); Require(await userManager.UpdateSecurityStampAsync(u)); TempData["Success"] = "Tài khoản có chứng từ đã được vô hiệu hóa; giữ nguyên người lập và lịch sử."; }
        else
        {
            var c = await db.Customers.SingleOrDefaultAsync(x => x.UserId == id); if (c is not null) db.Customers.Remove(c);
            await db.SaveChangesAsync(); Require(await userManager.RemoveFromRolesAsync(u, await userManager.GetRolesAsync(u))); Require(await userManager.RemoveClaimsAsync(u, await userManager.GetClaimsAsync(u)));
            db.UserLogins.RemoveRange(await db.UserLogins.Where(x => x.UserId == id).ToListAsync()); db.UserTokens.RemoveRange(await db.UserTokens.Where(x => x.UserId == id).ToListAsync()); await db.SaveChangesAsync(); Require(await userManager.DeleteAsync(u)); TempData["Success"] = "Đã xóa tài khoản chưa có chứng từ.";
        }
        await tx.CommitAsync(); return RedirectToAction(nameof(Index));
    }
}
