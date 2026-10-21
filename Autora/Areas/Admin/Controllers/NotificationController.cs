using System.Security.Claims;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class NotificationController(AdminNotificationService notifications) : Controller
{
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet] public async Task<IActionResult> Index(int page = 1, bool unreadOnly = false) => View(await notifications.ListAsync(CurrentUserId, page, unreadOnly));
    [HttpGet] public async Task<IActionResult> Feed() => Json(await notifications.ListAsync(CurrentUserId));
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Read(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 100) return BadRequest();
        var url = await notifications.MarkReadAsync(CurrentUserId, key);
        return url is null ? NotFound() : Json(new { url });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 100) return BadRequest();
        var url = await notifications.MarkReadAsync(CurrentUserId, key);
        return url is null ? NotFound() : LocalRedirect(url);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReadAll()
    { await notifications.MarkAllReadAsync(CurrentUserId); return NoContent(); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReadAllPage()
    { await notifications.MarkAllReadAsync(CurrentUserId); return RedirectToAction(nameof(Index)); }
}
