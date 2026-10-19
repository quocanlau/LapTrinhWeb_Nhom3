using Autora.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Autora.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Staff)] // Vá lỗ hổng: Ngăn chặn người dùng vãng lai truy cập
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Cài đặt hệ thống";
            ViewData["ActiveMenu"] = "Settings";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveShowroom()
        {
            TempData["Success"] = "Đã lưu thông tin cấu hình Showroom thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}