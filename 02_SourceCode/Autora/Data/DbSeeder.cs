using Autora.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Autora.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config, bool forceReset = false)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();

        if (db.Database.IsSqlServer())
        {
            await db.Database.MigrateAsync();
        }

        // 1. Seed Roles
        foreach (var name in new[] { Roles.Admin, Roles.Employee, Roles.Customer })
        {
            if (!await roles.RoleExistsAsync(name))
            {
                var r = await roles.CreateAsync(new IdentityRole(name));
                if (!r.Succeeded) throw new InvalidOperationException(string.Join("; ", r.Errors.Select(x => x.Description)));
            }
        }

        // 2. Seed Admin User
        // Configuration can be overridden by User Secrets/environment variables.
        // Never pass a PowerShell command or another malformed value to Identity.
        var configuredAdminEmail = config["Bootstrap:AdminEmail"]?.Trim();
        var adminEmail = IsValidEmail(configuredAdminEmail)
            ? configuredAdminEmail!
            : "admin@autora.local";
        var adminPassword = config["Bootstrap:AdminPassword"];
        if (string.IsNullOrWhiteSpace(adminPassword) || adminPassword.Contains("Read-Host", StringComparison.OrdinalIgnoreCase))
        {
            adminPassword = "Admin@12345";
        }
        var admin = await users.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Quản trị Autora",
                EmailConfirmed = true,
                IsActive = true
            };
            var r = await users.CreateAsync(admin, adminPassword);
            if (!r.Succeeded) throw new InvalidOperationException(string.Join("; ", r.Errors.Select(x => x.Description)));
        }
        if (!await users.IsInRoleAsync(admin, Roles.Admin))
        {
            await users.AddToRoleAsync(admin, Roles.Admin);
        }

        static bool IsValidEmail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 256)
            {
                return false;
            }

            try
            {
                return new System.Net.Mail.MailAddress(value).Address == value;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // 3. Seed Demo Customer User
        var customerEmail = "an@gmail.com";
        var customerUser = await users.FindByEmailAsync(customerEmail);
        if (customerUser is null)
        {
            customerUser = new ApplicationUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                FullName = "Lưu An",
                PhoneNumber = "0123456789",
                EmailConfirmed = true,
                IsActive = true
            };
            var r = await users.CreateAsync(customerUser, "Customer@12345");
            if (r.Succeeded)
            {
                await users.AddToRoleAsync(customerUser, Roles.Customer);
            }
        }
        if (customerUser is not null && !await db.Customers.AnyAsync(x => x.UserId == customerUser.Id))
        {
            db.Customers.Add(new Customer
            {
                UserId = customerUser.Id,
                FullName = "Lưu An",
                Phone = "0123456789",
                Email = customerEmail
            });
            await db.SaveChangesAsync();
        }

        // 4. Seed Showroom Settings
        if (!await db.ShowroomSettings.AnyAsync())
        {
            db.ShowroomSettings.Add(new ShowroomSetting
            {
                Id = 1,
                Name = "AUTORA MOTORS",
                Hotline = "1900 8888",
                Email = "contact@autora.vn",
                Address = "Hà Nội",
                OpeningHours = "08:00 - 20:00",
                Policy = "Cọc 2% giá bán chốt. Hoàn đủ tiền đã thu trước khi hủy."
            });
            await db.SaveChangesAsync();
        }

        // 5. Check if cars already exist (skip seeding if already populated)
        if (!forceReset && await db.Cars.AnyAsync())
        {
            await RepairSeededCarImagePathsAsync(db);
            return;
        }

        // 6. Seed Manufacturers, Models, Supplier, Receipt, Cars & Images
        // NOTE: All helpers run BEFORE BeginTransactionAsync to avoid nested SaveChanges inside tx.

        // Seed Manufacturers
        Manufacturer GetOrAddMfr(string name, string country)
        {
            var existing = db.Manufacturers.Local.FirstOrDefault(x => x.Name == name)
                ?? db.Manufacturers.FirstOrDefault(x => x.Name == name);
            if (existing is not null) return existing;
            var m = new Manufacturer { Name = name, Country = country, IsActive = true };
            db.Manufacturers.Add(m);
            return m;
        }
        var mToyota = GetOrAddMfr("Toyota", "Nhật Bản");
        var mFord = GetOrAddMfr("Ford", "Hoa Kỳ");
        var mHonda = GetOrAddMfr("Honda", "Nhật Bản");
        var mMitsubishi = GetOrAddMfr("Mitsubishi", "Nhật Bản");
        await db.SaveChangesAsync(); // persist manufacturers so they get IDs

        // Seed CarModels
        CarModel GetOrAddMod(Manufacturer mfr, string name, string bodyType)
        {
            var existing = db.CarModels.Local.FirstOrDefault(x => x.ManufacturerId == mfr.Id && x.Name == name)
                ?? db.CarModels.FirstOrDefault(x => x.ManufacturerId == mfr.Id && x.Name == name);
            if (existing is not null) return existing;
            var m = new CarModel { ManufacturerId = mfr.Id, Name = name, BodyType = bodyType, IsActive = true };
            db.CarModels.Add(m);
            return m;
        }
        var modCamry = GetOrAddMod(mToyota, "Camry", "Sedan");
        var modCorollaHEV = GetOrAddMod(mToyota, "Corolla Cross 1.8HEV", "SUV");
        var modCorollaV = GetOrAddMod(mToyota, "Corolla Cross 1.8V", "SUV");
        var modVios = GetOrAddMod(mToyota, "Vios 1.5E CVT", "Sedan");
        var modYaris = GetOrAddMod(mToyota, "Yaris Cross", "SUV");
        var modYarisHEV = GetOrAddMod(mToyota, "Yaris Cross HEV", "SUV");
        var modTerritoryTrend = GetOrAddMod(mFord, "Territory Trend 1.5L AT", "SUV");
        var modTerritoryTitanium = GetOrAddMod(mFord, "Territory Titanium 1.5L AT", "SUV");
        var modTerritoryTitaniumX = GetOrAddMod(mFord, "Territory Titanium X 1.5L AT", "SUV");
        var modCity = GetOrAddMod(mHonda, "City G", "Sedan");
        var modAttrageMT = GetOrAddMod(mMitsubishi, "Attrage MT", "Sedan");
        var modAttrageCVT = GetOrAddMod(mMitsubishi, "Attrage CVT Premium", "Sedan");
        var modXpanderMT = GetOrAddMod(mMitsubishi, "Xpander MT", "MPV");
        var modXpanderAT = GetOrAddMod(mMitsubishi, "Xpander AT", "MPV");
        var modXpanderATPrem = GetOrAddMod(mMitsubishi, "Xpander AT Premium", "MPV");
        await db.SaveChangesAsync(); // persist models so they get IDs

        // Seed Supplier
        var supplier = db.Suppliers.Local.FirstOrDefault(x => x.Name == "Đối tác dữ liệu mẫu")
            ?? db.Suppliers.FirstOrDefault(x => x.Name == "Đối tác dữ liệu mẫu");
        if (supplier is null)
        {
            supplier = new Supplier { Name = "Đối tác dữ liệu mẫu", SupplierType = "Tổ chức", Phone = "0900000000", IsActive = true };
            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();
        }

        // Now open transaction only for Cars + Receipt
        await using var tx = await db.Database.BeginTransactionAsync();

        var receipt = new PurchaseReceipt { SupplierId = supplier.Id, EmployeeId = admin.Id, Status = PurchaseStatus.Confirmed };

        var carDefs = new (CarModel model, string vin, short year, string color, CarCondition condition, int mileage, string fuel, string trans, decimal price, string image)[]
        {
            (modVios,           "ATRA0000000000001", 2024, "Đen",   CarCondition.New,  0,     "Xăng",   "CVT",           488_000_000m, "/images/cars/toyota-vios.jpg"),
            (modYaris,          "ATRA0000000000002", 2024, "Đen",   CarCondition.New,  0,     "Xăng",   "CVT",           650_000_000m, "/images/cars/toyota-yaris-cross-c-230920-05.jpg"),
            (modYarisHEV,       "ATRA0000000000003", 2024, "Đen",   CarCondition.New,  0,     "Hybrid", "CVT",           728_000_000m, "/images/cars/toyota-yaris-cross.jpg"),
            (modCorollaV,       "ATRA0000000000004", 2024, "Đen",   CarCondition.New,  0,     "Xăng",   "CVT",           820_000_000m, "/images/cars/toyota-corolla-cross.jpg"),
            (modCorollaHEV,     "ATRA0000000000005", 2024, "Đen",   CarCondition.New,  0,     "Hybrid", "e-CVT",         865_000_000m, "/images/cars/toyota-corolla-cross-19943.webp"),
            (modCity,           "ATRA0000000000006", 2024, "Đen",   CarCondition.New,  0,     "Xăng",   "CVT",           499_000_000m, "/images/cars/honda-city.png"),
            (modXpanderMT,      "ATRA0000000000007", 2024, "Trắng", CarCondition.New,  0,     "Xăng",   "Số sàn 5 cấp", 568_000_000m, "/images/cars/2135235-2139205j29545.webp"),
            (modXpanderAT,      "ATRA0000000000008", 2024, "Trắng", CarCondition.New,  0,     "Xăng",   "Tự động 4 cấp",598_000_000m, "/images/cars/9y0tzmv-2885400j33687.webp"),
            (modXpanderATPrem,  "ATRA0000000000009", 2024, "Đen",   CarCondition.New,  0,     "Xăng",   "Tự động 4 cấp",659_000_000m, "/images/cars/xpanderpreat.jpg"),
            (modAttrageMT,      "ATRA0000000000010", 2024, "Trắng", CarCondition.New,  0,     "Xăng",   "Số sàn 5 cấp", 380_000_000m, "/images/cars/mitsubishiattragemt.jpg"),
            (modAttrageCVT,     "ATRA0000000000011", 2024, "Xám",   CarCondition.New,  0,     "Xăng",   "CVT",           490_000_000m, "/images/cars/mitsubishi-attrage-cvt-premium-2236481j32336.webp"),
            (modTerritoryTrend, "ATRA0000000000012", 2024, "Đen",   CarCondition.New,  0,     "Xăng",   "Tự động 7 cấp",739_000_000m, "/images/cars/territory.jpg"),
            (modTerritoryTitanium, "ATRA0000000000013", 2024, "Đen",CarCondition.New,  0,     "Xăng",   "Tự động 7 cấp",819_000_000m, "/images/cars/ford-territory.jpg"),
            (modTerritoryTitaniumX,"ATRA0000000000014", 2024, "Đen",CarCondition.New,  0,     "Xăng",   "Tự động 7 cấp",875_000_000m, "/images/cars/territory-den-dau-xe-trai-truoc.jpg"),
            (modCamry,          "ATRA0000000000015", 2024, "Đỏ",   CarCondition.Used, 15000, "Hybrid", "Tự động",       950_000_000m, "/images/cars/2024-toyota-camry-102-64cbc4858e198.jpg"),
            (modCamry,          "ATRA0000000000016", 2024, "Trắng", CarCondition.New,  0,     "Xăng",   "Tự động",       960_000_000m, "/images/cars/camrytrang.jpg")
        };

        foreach (var def in carDefs)
        {
            if (db.Cars.Local.Any(x => x.VIN == def.vin)) continue;
            var car = new Car
            {
                CarModelId = def.model.Id,
                VIN = def.vin,
                ManufactureYear = def.year,
                Color = def.color,
                Condition = def.condition,
                Mileage = def.mileage,
                Fuel = def.fuel,
                Transmission = def.trans,
                ListedPrice = def.price,
                Status = CarStatus.Available,
                IsPublished = true
            };
            car.Images.Add(new CarImage { ImagePath = def.image, IsPrimary = true, SortOrder = 0 });
            receipt.Details.Add(new PurchaseDetail { Car = car, PurchasePrice = def.price - 50_000_000m });
        }

        db.PurchaseReceipts.Add(receipt);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private static async Task RepairSeededCarImagePathsAsync(AppDbContext db)
    {
        var imageByModel = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Attrage CVT Premium"] = "/images/cars/mitsubishi-attrage-cvt-premium-2236481j32336.webp",
            ["Xpander MT"] = "/images/cars/2135235-2139205j29545.webp",
            ["Xpander AT"] = "/images/cars/9y0tzmv-2885400j33687.webp",
            ["Xpander AT Premium"] = "/images/cars/xpanderpreat.jpg",
            ["Camry"] = "/images/cars/2024-toyota-camry-102-64cbc4858e198.jpg",
            ["Corolla Cross 1.8HEV"] = "/images/cars/toyota-corolla-cross-19943.webp",
            ["Corolla Cross 1.8V"] = "/images/cars/toyota-corolla-cross.jpg",
            ["Vios 1.5E CVT"] = "/images/cars/toyota-vios.jpg",
            ["Yaris Cross"] = "/images/cars/toyota-yaris-cross-c-230920-05.jpg",
            ["Yaris Cross HEV"] = "/images/cars/toyota-yaris-cross.jpg",
            ["City G"] = "/images/cars/honda-city.png",
            ["Attrage MT"] = "/images/cars/mitsubishiattragemt.jpg",
            ["Territory Trend 1.5L AT"] = "/images/cars/territory.jpg",
            ["Territory Titanium 1.5L AT"] = "/images/cars/ford-territory.jpg",
            ["Territory Titanium X 1.5L AT"] = "/images/cars/territory-den-dau-xe-trai-truoc.jpg"
        };

        var cars = await db.Cars
            .Include(x => x.CarModel)
            .Include(x => x.Images)
            .ToListAsync();

        var changed = false;
        foreach (var car in cars)
        {
            if (!imageByModel.TryGetValue(car.CarModel.Name, out var expectedPath))
            {
                continue;
            }

            var primary = car.Images.OrderByDescending(x => x.IsPrimary).ThenBy(x => x.SortOrder).FirstOrDefault();
            if (primary is null)
            {
                car.Images.Add(new CarImage { ImagePath = expectedPath, IsPrimary = true, SortOrder = 0 });
                changed = true;
            }
            else if (!string.Equals(primary.ImagePath, expectedPath, StringComparison.OrdinalIgnoreCase))
            {
                primary.ImagePath = expectedPath;
                changed = true;
            }
        }

        if (changed)
        {
            await db.SaveChangesAsync();
        }
    }
}
