using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"), Authorize(Roles = Roles.Staff)]
public class ContactController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(int page = 1) => View(await db.ContactInquiries.AsNoTracking().OrderByDescending(x => x.Id).PageAsync(page));
}
