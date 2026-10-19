using System.ComponentModel.DataAnnotations;
using Autora.Data;
using Autora.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers.Api;
public class ManufacturerDto
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [StringLength(100)] public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
}
[ApiController, Route("api/manufacturers"), Route("api/brands")]
public class ManufacturersController(AppDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok(await db.Manufacturers.AsNoTracking().Where(x => x.IsActive).Select(x => new { x.Id, x.Name, x.Country }).ToListAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var m = await db.Manufacturers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && (x.IsActive || User.IsInRole(Roles.Admin))); return m is null ? NotFound() : Ok(new { m.Id, m.Name, m.Country }); }
    [HttpPost, Authorize(Roles = Roles.Admin)] public async Task<IActionResult> Create(ManufacturerDto dto) { var m = new Manufacturer { Name = dto.Name.Trim(), Country = dto.Country, IsActive = dto.IsActive }; db.Manufacturers.Add(m); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = m.Id }, new { m.Id, m.Name, m.Country, m.IsActive }); }
    [HttpPut("{id:int}"), Authorize(Roles = Roles.Admin)] public async Task<IActionResult> Update(int id, ManufacturerDto dto) { var m = await db.Manufacturers.FindAsync(id); if (m is null) return NotFound(); m.Name = dto.Name.Trim(); m.Country = dto.Country; m.IsActive = dto.IsActive; await db.SaveChangesAsync(); return NoContent(); }
    [HttpDelete("{id:int}"), Authorize(Roles = Roles.Admin)] public async Task<IActionResult> Delete(int id) { var m = await db.Manufacturers.FindAsync(id); if (m is null) return NotFound(); if (await db.CarModels.AnyAsync(x => x.ManufacturerId == id)) return Conflict(new { message = "Hãng đã có dòng xe." }); db.Manufacturers.Remove(m); await db.SaveChangesAsync(); return NoContent(); }
}
