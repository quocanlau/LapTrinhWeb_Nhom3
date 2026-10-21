using Autora.Data;
using Autora.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Autora.Controllers;
public class ContactController(AppDbContext db) : Controller
{
    public IActionResult Index(string? email = null) => View(new ContactInquiry { Email = email });
    public IActionResult Thanks() => View();
    [HttpPost, EnableRateLimiting("contact")]
    public async Task<IActionResult> Save([Bind("FullName,Phone,Email,InterestedModel,Message")] ContactInquiry form)
    {
        if (string.IsNullOrWhiteSpace(form.Phone) && string.IsNullOrWhiteSpace(form.Email)) ModelState.AddModelError("", "Cần điện thoại hoặc email để showroom liên hệ.");
        if (!ModelState.IsValid) return View("Index", form);
        form.CreatedAt = DateTimeOffset.UtcNow; db.ContactInquiries.Add(form); await db.SaveChangesAsync(); return RedirectToAction(nameof(Thanks));
    }
}
