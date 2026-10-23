using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Autora.Controllers;
public class AccountController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, AppDbContext db) : Controller
{
    [HttpGet, AllowAnonymous] public IActionResult Login(string? returnUrl) => View(new LoginForm { ReturnUrl = returnUrl });
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login(LoginForm form)
    {
        if (!ModelState.IsValid) return View(form);
        var user = await users.FindByEmailAsync(form.Email);
        if (user is null || !user.IsActive) { ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ."); return View(form); }
        var result = await signIn.PasswordSignInAsync(user, form.Password, form.RememberMe, true);
        if (!result.Succeeded) { ModelState.AddModelError("", result.IsLockedOut ? "Tài khoản tạm khóa. Hãy thử lại sau." : "Thông tin đăng nhập không hợp lệ."); return View(form); }
        var isStaff = await users.IsInRoleAsync(user, Roles.Admin) || await users.IsInRoleAsync(user, Roles.Employee);
        if (isStaff)
        {
            if (!string.IsNullOrEmpty(form.ReturnUrl) && Url.IsLocalUrl(form.ReturnUrl))
            {
                return LocalRedirect(form.ReturnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        else
        {
            if (!string.IsNullOrEmpty(form.ReturnUrl) && Url.IsLocalUrl(form.ReturnUrl) && !form.ReturnUrl.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
            {
                return LocalRedirect(form.ReturnUrl);
            }
            return RedirectToAction("Index", "Car");
        }
    }
    [HttpGet, AllowAnonymous] public IActionResult Register() => View(new RegisterForm());
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register(RegisterForm form)
    {
        if (!ModelState.IsValid) return View(form);
        await using var tx = await db.Database.BeginTransactionAsync();
        var user = new ApplicationUser { UserName = form.Email, Email = form.Email, FullName = form.FullName, PhoneNumber = form.Phone };
        var result = await users.CreateAsync(user, form.Password);
        if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError("", e.Description); return View(form); }
        result = await users.AddToRoleAsync(user, Roles.Customer);
        if (!result.Succeeded) throw new InvalidOperationException("Chưa có vai trò Customer. Hãy chạy seed dữ liệu.");
        db.Customers.Add(new Customer { UserId = user.Id, FullName = form.FullName, Phone = form.Phone, Email = form.Email });
        await db.SaveChangesAsync(); await tx.CommitAsync(); await signIn.SignInAsync(user, false);
        return RedirectToAction("Index", "Car");
    }
    [HttpPost, Authorize] public async Task<IActionResult> Logout() { await signIn.SignOutAsync(); HttpContext.Session.Clear(); return RedirectToAction("Index", "Home"); }
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View();
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> Profile()
    {
        var u = await users.GetUserAsync(User); if (u is null) return Challenge();
        var c = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(db.Customers, x => x.UserId == u.Id);
        return View(new ProfileForm { FullName = u.FullName, Phone = u.PhoneNumber ?? "", Address = c?.Address });
    }
    [Authorize, HttpPost]
    public async Task<IActionResult> Profile(ProfileForm f)
    {
        if (!ModelState.IsValid) return View(f);
        var u = await users.GetUserAsync(User); if (u is null) return Challenge();
        await using var tx = await db.Database.BeginTransactionAsync();
        u.FullName = f.FullName.Trim(); u.PhoneNumber = f.Phone.Trim();
        var result = await users.UpdateAsync(u);
        if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError("", e.Description); return View(f); }
        var c = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(db.Customers, x => x.UserId == u.Id);
        if (c is not null) { c.FullName = u.FullName; c.Phone = u.PhoneNumber; c.Address = f.Address; }
        await db.SaveChangesAsync(); await tx.CommitAsync(); await signIn.RefreshSignInAsync(u);
        TempData["Message"] = "Đã cập nhật hồ sơ."; return RedirectToAction(nameof(Profile));
    }
    [Authorize, HttpGet] public IActionResult ChangePassword() => View(new PasswordForm());
    [Authorize, HttpPost]
    public async Task<IActionResult> ChangePassword(PasswordForm f)
    {
        if (!ModelState.IsValid) return View(f);
        var u = await users.GetUserAsync(User); if (u is null) return Challenge();
        var result = await users.ChangePasswordAsync(u, f.CurrentPassword, f.NewPassword);
        if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError("", e.Description); return View(f); }
        await signIn.RefreshSignInAsync(u); TempData["Message"] = "Đã đổi mật khẩu."; return RedirectToAction(nameof(Profile));
    }
}
