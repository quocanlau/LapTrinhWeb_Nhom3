using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Autora.Controllers.Api;
[ApiController,Route("api/cars")]
public class CarsController(CarQuery query,AppDbContext db):ControllerBase
{
    private static object PublicCar(Car c)=>new{id=c.Id,manufacturer=c.CarModel.Manufacturer.Name,model=c.CarModel.Name,c.ManufactureYear,c.Color,c.Mileage,c.Fuel,c.Transmission,c.Condition,price=c.ListedPrice,deposit=DepositPolicy.Calculate(c.ListedPrice),status=c.Status.ToString(),canRequestDeposit=c.Status==CarStatus.Available,images=c.Images.Select(x=>x.ImagePath)};
    [HttpGet] public async Task<IActionResult> List([FromQuery]CarFilter filter){var result=await query.ListAsync(filter);return Ok(new{items=result.Cars.Select(PublicCar),result.TotalCount,page=result.Filter.Page,pageSize=result.Filter.PageSize});}
    [HttpGet("{id:int}")] public async Task<IActionResult> Details(int id){var c=await db.Cars.AsNoTracking().Include(x=>x.CarModel).ThenInclude(x=>x.Manufacturer).Include(x=>x.Images).SingleOrDefaultAsync(x=>x.Id==id && x.IsPublished && x.Status!=CarStatus.AwaitingImport && x.Status!=CarStatus.Sold && x.CarModel.IsActive && x.CarModel.Manufacturer.IsActive);return c is null?NotFound():Ok(PublicCar(c));}
}
