using Autora.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Autora.Data;
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services,IConfiguration config)
    {
        var db=services.GetRequiredService<AppDbContext>();
        var users=services.GetRequiredService<UserManager<ApplicationUser>>();
        var roles=services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach(var name in new[]{Roles.Admin,Roles.Employee,Roles.Customer})
            if(!await roles.RoleExistsAsync(name)){var r=await roles.CreateAsync(new IdentityRole(name));if(!r.Succeeded)throw new InvalidOperationException(string.Join("; ",r.Errors.Select(x=>x.Description)));}
        var email=config["Bootstrap:AdminEmail"];
        var password=config["Bootstrap:AdminPassword"];
        if(string.IsNullOrWhiteSpace(email)||string.IsNullOrWhiteSpace(password))throw new InvalidOperationException("Đặt Bootstrap:AdminEmail và Bootstrap:AdminPassword bằng user-secrets hoặc biến môi trường trước khi chạy --seed.");
        var admin=await users.FindByEmailAsync(email);
        if(admin is null){admin=new ApplicationUser{UserName=email,Email=email,FullName="Quản trị Autora",EmailConfirmed=true};var r=await users.CreateAsync(admin,password);if(!r.Succeeded)throw new InvalidOperationException(string.Join("; ",r.Errors.Select(x=>x.Description)));}
        if(!await users.IsInRoleAsync(admin,Roles.Admin)){var r=await users.AddToRoleAsync(admin,Roles.Admin);if(!r.Succeeded)throw new InvalidOperationException(string.Join("; ",r.Errors.Select(x=>x.Description)));}
        if(!config.GetValue<bool>("Seed:IncludeDemoData") || await db.Cars.AnyAsync())return;
        await using var tx=await db.Database.BeginTransactionAsync();
        var manufacturer=await db.Manufacturers.FirstOrDefaultAsync(x=>x.Name=="Toyota");
        if(manufacturer is null){manufacturer=new Manufacturer{Name="Toyota",Country="Nhật Bản"};db.Manufacturers.Add(manufacturer);await db.SaveChangesAsync();}
        var model=await db.CarModels.FirstOrDefaultAsync(x=>x.ManufacturerId==manufacturer.Id && x.Name=="Camry");
        if(model is null){model=new CarModel{ManufacturerId=manufacturer.Id,Name="Camry",BodyType="Sedan"};db.CarModels.Add(model);await db.SaveChangesAsync();}
        var supplier=new Supplier{Name="Đối tác dữ liệu mẫu",SupplierType="Tổ chức",Phone="0900000000"};
        var receipt=new PurchaseReceipt{Supplier=supplier,EmployeeId=admin.Id,Status=PurchaseStatus.Confirmed};
        for(var i=1;i<=16;i++){
            var car=new Car{CarModelId=model.Id,VIN="ATRA"+i.ToString("D13"),ManufactureYear=2024,Color=i%2==0?"Trắng":"Đen",
                Condition=i%2==0?CarCondition.New:CarCondition.Used,Mileage=i%2==0?0:i*1000,Fuel=i%3==0?"Hybrid":"Xăng",Transmission="Tự động",
                ListedPrice=800_000_000m+i*10_000_000m,Status=CarStatus.Available,IsPublished=true};
            var imgPath = i % 2 == 0 ? "/images/cars/camrytrang.jpg" : "/images/cars/2024-toyota-camry-102-64cbc4858e198.jpg";
            car.Images.Add(new CarImage{ImagePath=imgPath,IsPrimary=true});
            receipt.Details.Add(new PurchaseDetail{Car=car,PurchasePrice=car.ListedPrice-100_000_000m});
        }
        db.PurchaseReceipts.Add(receipt);await db.SaveChangesAsync();await tx.CommitAsync();
    }
}
