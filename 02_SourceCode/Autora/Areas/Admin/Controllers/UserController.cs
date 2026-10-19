using Autora.Data;
using Autora.Models;
using Autora.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autora.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class UserController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext db) : Controller
    {
        // GET: /Admin/User
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý tài khoản & Phân quyền";
            ViewData["ActiveMenu"] = "Users";

            var currentUserId = userManager.GetUserId(User);
            var currentUserName = User.Identity?.Name;

            var roleMap = await db.Roles.AsNoTracking().ToDictionaryAsync(r => r.Id, r => r.Name ?? "");
            var userRolesList = await db.UserRoles.AsNoTracking().ToListAsync();
            var allUsers = await db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync();

            var userRoleLookup = userRolesList
                .GroupBy(ur => ur.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ur => roleMap.TryGetValue(ur.RoleId, out var rName) ? rName : "").Where(r => !string.IsNullOrEmpty(r)).ToList()
                );

            // Determine which users have active financial or order transactions
            var customerUserIdsWithOrders = await (
                from c in db.Customers
                where db.SaleTransactions.Any(s => s.CustomerId == c.Id) || db.DepositRequests.Any(r => r.CustomerId == c.Id)
                select c.UserId
            ).Where(uId => uId != null).ToListAsync();

            var staffUserIdsWithTransactions = await (
                from u in db.Users
                where db.SaleTransactions.Any(s => s.EmployeeId == u.Id)
                   || db.PurchaseReceipts.Any(p => p.EmployeeId == u.Id)
                   || db.PaymentEntries.Any(p => p.CreatedById == u.Id)
                select u.Id
            ).ToListAsync();

            var usersWithTransactions = new HashSet<string>(customerUserIdsWithOrders!.Concat(staffUserIdsWithTransactions));

            int index = 1;
            var list = new List<UserItemViewModel>();
            foreach (var u in allUsers)
            {
                var rolesOfUser = userRoleLookup.TryGetValue(u.Id, out var rList) ? rList : [];
                var roleName = rolesOfUser.Contains(Roles.Admin) ? Roles.Admin :
                               rolesOfUser.Contains(Roles.Employee) ? Roles.Employee :
                               rolesOfUser.Contains(Roles.Customer) ? Roles.Customer : "Chưa phân quyền";

                list.Add(new UserItemViewModel
                {
                    Id = u.Id,
                    Code = $"NV-{index:D3}",
                    FullName = u.FullName,
                    Email = u.Email ?? u.UserName ?? "",
                    Phone = u.PhoneNumber ?? "",
                    Role = roleName,
                    IsActive = u.IsActive,
                    IsCurrentAdmin = u.Id == currentUserId || string.Equals(u.UserName, currentUserName, StringComparison.OrdinalIgnoreCase),
                    Initials = GetInitials(u.FullName),
                    HasTransactions = usersWithTransactions.Contains(u.Id)
                });
                index++;
            }

            var model = new UserListViewModel { Users = list };
            return View(model);
        }

        // POST: /Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateForm form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Index));
            }

            var existing = await userManager.FindByEmailAsync(form.Email);
            if (existing != null)
            {
                TempData["Error"] = $"Email '{form.Email}' đã được sử dụng trong hệ thống.";
                return RedirectToAction(nameof(Index));
            }

            // Only allow assigning Employee or Admin role
            var targetRole = form.Role == Roles.Admin ? Roles.Admin : Roles.Employee;

            if (!await roleManager.RoleExistsAsync(targetRole))
            {
                await roleManager.CreateAsync(new IdentityRole(targetRole));
            }

            var password = string.IsNullOrWhiteSpace(form.Password) ? "Autora@2026" : form.Password;
            var user = new ApplicationUser
            {
                UserName = form.Email,
                Email = form.Email,
                FullName = form.FullName.Trim(),
                PhoneNumber = form.Phone.Trim(),
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }

            await userManager.AddToRoleAsync(user, targetRole);

            TempData["Success"] = $"Tạo tài khoản cho '{user.FullName}' ({(targetRole == Roles.Admin ? "Quản trị viên" : "Nhân viên")}) thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/User/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Mã tài khoản không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = userManager.GetUserId(User);
            if (user.Id == currentUserId || string.Equals(user.UserName, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Bạn không thể tự khóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            if (user.IsActive && await userManager.IsInRoleAsync(user, Roles.Admin))
            {
                var adminUsers = await userManager.GetUsersInRoleAsync(Roles.Admin);
                if (adminUsers.Count(a => a.IsActive) <= 1)
                {
                    TempData["Error"] = "Không thể vô hiệu hóa quản trị viên duy nhất còn lại của hệ thống.";
                    return RedirectToAction(nameof(Index));
                }
            }

            user.IsActive = !user.IsActive;
            if (!user.IsActive)
            {
                // Invalidate active session stamps when locking
                await userManager.UpdateSecurityStampAsync(user);
            }

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Đã {(user.IsActive ? "kích hoạt mở khóa" : "tạm khóa")} tài khoản '{user.FullName}'!";
            }
            else
            {
                TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/User/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Mã tài khoản không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, "Autora@2026");
            if (result.Succeeded)
            {
                await userManager.UpdateSecurityStampAsync(user);
                TempData["Success"] = $"Đã đặt lại mật khẩu cho '{user.FullName}' về mặc định (Autora@2026)!";
            }
            else
            {
                TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/User/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Mã tài khoản không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = userManager.GetUserId(User);
            if (user.Id == currentUserId || string.Equals(user.UserName, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Bạn không thể xóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            if (await userManager.IsInRoleAsync(user, Roles.Admin))
            {
                var adminUsers = await userManager.GetUsersInRoleAsync(Roles.Admin);
                if (adminUsers.Count <= 1)
                {
                    TempData["Error"] = "Không thể xóa quản trị viên duy nhất còn lại.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                // 1. If this employee has any sale transactions, purchase receipts, or payment entries,
                // reassign them to the current managing Admin so business history is preserved
                var sales = await db.SaleTransactions.Where(s => s.EmployeeId == user.Id).ToListAsync();
                foreach (var s in sales) s.EmployeeId = currentUserId!;

                var purchases = await db.PurchaseReceipts.Where(p => p.EmployeeId == user.Id).ToListAsync();
                foreach (var p in purchases) p.EmployeeId = currentUserId!;

                var payments = await db.PaymentEntries.Where(p => p.CreatedById == user.Id).ToListAsync();
                foreach (var p in payments) p.CreatedById = currentUserId!;

                // 2. If this user is linked to a Customer record:
                var customer = await db.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (customer != null)
                {
                    var hasCustomerOrders = await db.SaleTransactions.AnyAsync(s => s.CustomerId == customer.Id)
                                         || await db.DepositRequests.AnyAsync(r => r.CustomerId == customer.Id);
                    if (hasCustomerOrders)
                    {
                        // Unlink account from customer profile to preserve buyer transaction history
                        customer.UserId = null;
                    }
                    else
                    {
                        db.Customers.Remove(customer);
                    }
                }

                await db.SaveChangesAsync();

                // 3. Clear Identity role and claim mappings to avoid FK_AspNetUserRoles Restrict constraint
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Count > 0)
                {
                    await userManager.RemoveFromRolesAsync(user, roles);
                }

                var claims = await userManager.GetClaimsAsync(user);
                if (claims.Count > 0)
                {
                    await userManager.RemoveClaimsAsync(user, claims);
                }

                // 4. Delete user
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Index));
                }

                await tx.CommitAsync();

                var handoverMsg = (sales.Count + purchases.Count + payments.Count) > 0 
                    ? $" (Đã bàn giao {sales.Count + purchases.Count + payments.Count} hồ sơ/phiếu giao dịch sang cho Quản trị viên)" 
                    : "";
                TempData["Success"] = $"Đã xóa vĩnh viễn tài khoản '{user.FullName}' ({user.Email}) thành công!{handoverMsg}";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                TempData["Error"] = $"Không thể xóa tài khoản '{user.FullName}': {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private static string GetInitials(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "AU";
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpperInvariant();
        }
    }
}
