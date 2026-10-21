using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Autora.Tests;
public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute() { if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AUTORA_TEST_SQLSERVER"))) Skip = "Chỉ chạy khi đặt AUTORA_TEST_SQLSERVER. Tạo và xóa database test riêng."; }
}
public class SqlServerIntegrationTests
{
    [SqlServerFact]
    public async Task RealSqlServerPreventsDuplicateReservationsAndOverpayment()
    {
        var builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("AUTORA_TEST_SQLSERVER")) { InitialCatalog = "Autora_Integration_" + Guid.NewGuid().ToString("N") };
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(builder.ConnectionString).Options;
        await using var setup = new AppDbContext(options);
        try
        {
            await setup.Database.MigrateAsync(); setup.Users.Add(new ApplicationUser { Id = "staff", UserName = "test", FullName = "Staff" }); setup.Customers.Add(new Customer { FullName = "Buyer", Phone = "0900000000" }); setup.Suppliers.Add(new Supplier { Name = "Seller" }); setup.Cars.Add(new Car { CarModel = new CarModel { Name = "Model", Manufacturer = new Manufacturer { Name = "Brand" } }, VIN = "TEST0000000000001", ManufactureYear = 2024, Color = "White", ListedPrice = 800000000, IsPublished = true }); await setup.SaveChangesAsync(); await new PurchaseService(setup).ImportAsync(1, 1, 600000000, "staff");
            var request = await new SaleService(setup).RequestAsync(1, 1, null);
            async Task<bool> Reserve() { await using var c = new AppDbContext(options); try { await new SaleService(c).ReserveAsync(request, 800000000, "staff"); return true; } catch (BusinessException) { return false; } catch (DbUpdateException) { return false; } catch (SqlException e) when (e.Number == 1205) { return false; } }
            var reservations = await Task.WhenAll(Reserve(), Reserve()); Assert.Single(reservations, x => x);
            await using var read = new AppDbContext(options); var sale = await read.SaleTransactions.SingleAsync(x => x.Status == SaleStatus.Reserved);
            async Task<bool> Pay() { await using var c = new AppDbContext(options); try { await new SaleService(c).PayAsync(sale.Id, PaymentType.Receipt, 800000000, "Cash", null, "staff"); return true; } catch (BusinessException) { return false; } catch (DbUpdateException) { return false; } catch (SqlException e) when (e.Number == 1205) { return false; } }
            var payments = await Task.WhenAll(Pay(), Pay()); Assert.Single(payments, x => x); Assert.Equal(800000000m, await read.PaymentEntries.SumAsync(x => x.Amount)); Assert.Single(await read.PaymentEntries.ToListAsync());
            var config = new ShowroomSetting { Name = "Test", Hotline = "0900000000", Email = "test@example.local", Address = "Test" }; read.ShowroomSettings.Add(config); await read.SaveChangesAsync(); Assert.Equal(1, config.Id);
        }
        finally
        {
            if (builder.InitialCatalog.StartsWith("Autora_Integration_", StringComparison.Ordinal)) await setup.Database.EnsureDeletedAsync();
        }
    }
}
