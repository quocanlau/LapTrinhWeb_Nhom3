using Autora.Data;
using Autora.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=Roles.Staff)]
public class HomeController(AppDbContext db):Controller
{
    public async Task<IActionResult> Index(){ViewBag.Cars=await db.Cars.CountAsync();ViewBag.Deposited=await db.Cars.CountAsync(x=>x.Status==CarStatus.Deposited);ViewBag.Sold=await db.Cars.CountAsync(x=>x.Status==CarStatus.Sold);ViewBag.Revenue=await db.SaleTransactions.Where(x=>x.Status==SaleStatus.Delivered).SumAsync(x=>(decimal?)x.AgreedPrice)??0m;ViewBag.Cash=await db.PaymentEntries.SumAsync(x=>(decimal?)(x.EntryType==PaymentType.Receipt?x.Amount:-x.Amount))??0m;return View();}
}
