using Autora.Data;
using Autora.Models;
using Autora.Services;
using Autora.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;
namespace Autora.Tests;
public class TestStore : IDisposable
{
    public SqliteConnection Connection { get; } = new("Data Source=:memory:");
    public TestContext Db { get; }
    public TestStore() { Connection.Open(); Db = NewContext(); Db.Database.EnsureCreated(); }
    public TestContext NewContext() => new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(Connection).Options);
    public async Task Seed()
    {
        var u = new ApplicationUser { Id = "staff", UserName = "staff@test.local", FullName = "Staff" };
        var m = new Manufacturer { Name = "Test" }; var model = new CarModel { Manufacturer = m, Name = "Model", BodyType = "SUV" };
        Db.Users.Add(u); Db.Customers.Add(new Customer { FullName = "Buyer", Phone = "0900000000" }); Db.Suppliers.Add(new Supplier { Name = "Seller" });
        for (int i = 1; i <= 3; i++) Db.Cars.Add(new Car { CarModel = model, VIN = "TEST" + i.ToString("D13"), ManufactureYear = 2024, Color = "Trắng", ListedPrice = 800000000, Status = CarStatus.AwaitingImport, IsPublished = true });
        await Db.SaveChangesAsync();
    }
    public void Dispose() { Db.Dispose(); Connection.Dispose(); }
}
public class TestContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        foreach (var e in b.Model.GetEntityTypes()) foreach (var p in e.GetProperties())
            {
                if (p.ClrType == typeof(decimal)) p.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<decimal, long>(v => decimal.ToInt64(v), v => v));
                if (p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?)) p.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, long>(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero)));
                if (p.ClrType == typeof(byte[]) && p.IsConcurrencyToken) { p.ValueGenerated = ValueGenerated.Never; p.SetColumnType("BLOB"); }
            }
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges(); foreach (var e in ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified).ToList()) foreach (var p in e.Properties.Where(x => x.Metadata.IsConcurrencyToken && x.Metadata.ClrType == typeof(byte[]))) p.CurrentValue = Guid.NewGuid().ToByteArray();
        return base.SaveChangesAsync(cancellationToken);
    }
}
public class WorkflowTests
{
    private static PurchaseDraftForm Form() => new() { SupplierId = 1, Lines = [new() { CarId = 1, PurchasePrice = 600000000 }, new() { CarId = 2, PurchasePrice = 650000000 }] };
    [Fact]
    public async Task MultiCarDraftDoesNotPublishUntilConfirmed()
    {
        using var s = new TestStore(); await s.Seed(); var service = new PurchaseService(s.Db); var id = await service.SaveDraftAsync(Form(), "staff");
        Assert.All(await s.Db.Cars.ToListAsync(), c => Assert.Equal(CarStatus.AwaitingImport, c.Status));
        var p = await s.Db.PurchaseReceipts.FindAsync(id); await service.ConfirmAsync(id, Convert.ToBase64String(p!.RowVersion));
        Assert.Equal(2, await s.Db.Cars.CountAsync(x => x.Status == CarStatus.Available)); Assert.Equal(PurchaseStatus.Confirmed, p.Status);
        await Assert.ThrowsAsync<BusinessException>(() => service.DeleteDraftAsync(id, Convert.ToBase64String(p.RowVersion)));
    }
    [Fact]
    public async Task ConfirmFailureRollsBackAllCars()
    {
        using var s = new TestStore(); await s.Seed(); var service = new PurchaseService(s.Db); var id = await service.SaveDraftAsync(Form(), "staff"); var p = await s.Db.PurchaseReceipts.FindAsync(id);
        var car = await s.Db.Cars.FindAsync(2); car!.Status = CarStatus.Sold; await s.Db.SaveChangesAsync();
        await Assert.ThrowsAsync<BusinessException>(() => service.ConfirmAsync(id, Convert.ToBase64String(p!.RowVersion)));
        using var verify = s.NewContext(); Assert.Equal(CarStatus.AwaitingImport, (await verify.Cars.FindAsync(1))!.Status); Assert.Equal(PurchaseStatus.Draft, (await verify.PurchaseReceipts.FindAsync(id))!.Status);
    }
    [Fact]
    public async Task DraftCanBeEditedAndDeletedWithoutImporting()
    {
        using var s = new TestStore(); await s.Seed(); var service = new PurchaseService(s.Db); var id = await service.SaveDraftAsync(Form(), "staff"); var p = await s.Db.PurchaseReceipts.FindAsync(id);
        var f = Form(); f.Id = id; f.Version = Convert.ToBase64String(p!.RowVersion); f.Lines.RemoveAt(1); await service.SaveDraftAsync(f, "staff"); Assert.Single(await s.Db.PurchaseDetails.ToListAsync());
        await service.DeleteDraftAsync(id, Convert.ToBase64String(p.RowVersion)); Assert.Empty(await s.Db.PurchaseDetails.ToListAsync()); Assert.All(await s.Db.Cars.ToListAsync(), c => Assert.Equal(CarStatus.AwaitingImport, c.Status));
    }
    [Fact]
    public async Task DuplicateCarsInPurchaseAreRejected()
    {
        using var s = new TestStore(); await s.Seed(); var f = Form(); f.Lines[1].CarId = 1; await Assert.ThrowsAsync<BusinessException>(() => new PurchaseService(s.Db).SaveDraftAsync(f, "staff")); Assert.Empty(await s.Db.PurchaseReceipts.ToListAsync());
    }
    [Fact]
    public async Task SaleDraftDoesNotReserveAndCannotTakePayment()
    {
        using var s = new TestStore(); await s.Seed(); await new PurchaseService(s.Db).ImportAsync(1, 1, 600000000, "staff"); var sale = new SaleService(s.Db); var id = await sale.SaveDraftAsync(new() { CarId = 1, CustomerId = 1, AgreedPrice = 800000000 }, "staff");
        Assert.Equal(CarStatus.Available, (await s.Db.Cars.FindAsync(1))!.Status); await Assert.ThrowsAsync<BusinessException>(() => sale.PayAsync(id, PaymentType.Receipt, 16000000, "Cash", null, "staff"));
    }
    [Fact]
    public async Task ReceiptRefundCancelPreserveLedger()
    {
        using var s = new TestStore(); await s.Seed(); await new PurchaseService(s.Db).ImportAsync(1, 1, 600000000, "staff"); var service = new SaleService(s.Db); var req = await service.RequestAsync(1, 1, null); var id = await service.ReserveAsync(req, 800000000, "staff");
        await service.PayAsync(id, PaymentType.Receipt, 16000000, "Cash", null, "staff"); await Assert.ThrowsAsync<BusinessException>(() => service.CancelAsync(id));
        await service.PayAsync(id, PaymentType.Refund, 16000000, "Cash", null, "staff"); await service.CancelAsync(id); Assert.Equal(2, await s.Db.PaymentEntries.CountAsync()); Assert.Equal(CarStatus.Available, (await s.Db.Cars.FindAsync(1))!.Status);
    }
    [Fact]
    public async Task DeliveryRequiresFullBalanceAndBlocksSecondDelivery()
    {
        using var s = new TestStore(); await s.Seed(); await new PurchaseService(s.Db).ImportAsync(1, 1, 600000000, "staff"); var service = new SaleService(s.Db); var req = await service.RequestAsync(1, 1, null); var id = await service.ReserveAsync(req, 800000000, "staff");
        await service.PayAsync(id, PaymentType.Receipt, 16000000, "Cash", null, "staff"); await Assert.ThrowsAsync<BusinessException>(() => service.DeliverAsync(id, "Buyer", null));
        await service.PayAsync(id, PaymentType.Receipt, 784000000, "Cash", null, "staff"); await service.DeliverAsync(id, "Buyer", null); Assert.Single(await s.Db.Deliveries.ToListAsync()); Assert.Equal(CarStatus.Sold, (await s.Db.Cars.FindAsync(1))!.Status); await Assert.ThrowsAsync<BusinessException>(() => service.DeliverAsync(id, "Buyer", null));
    }
    [Fact]
    public async Task TwoDraftsCanExistButOnlyOneCanReserve()
    {
        using var s = new TestStore(); await s.Seed(); await new PurchaseService(s.Db).ImportAsync(1, 1, 600000000, "staff"); var service = new SaleService(s.Db); var f = new SaleDraftForm { CarId = 1, CustomerId = 1, AgreedPrice = 800000000 }; var a = await service.SaveDraftAsync(f, "staff"); var b = await service.SaveDraftAsync(f, "staff");
        var first = await s.Db.SaleTransactions.FindAsync(a); await service.ConfirmDraftAsync(a, Convert.ToBase64String(first!.RowVersion)); var second = await s.Db.SaleTransactions.FindAsync(b); await Assert.ThrowsAsync<BusinessException>(() => service.ConfirmDraftAsync(b, Convert.ToBase64String(second!.RowVersion))); await service.CancelAsync(b); Assert.Equal(CarStatus.Reserved, (await s.Db.Cars.FindAsync(1))!.Status);
    }
    [Fact]
    public async Task StaleCarVersionIsRejectedAcrossContexts()
    {
        using var s = new TestStore(); await s.Seed(); using var a = s.NewContext(); using var b = s.NewContext(); var ca = await a.Cars.FindAsync(1); var cb = await b.Cars.FindAsync(1); ca!.Color = "Đen"; await a.SaveChangesAsync(); cb!.Color = "Đỏ"; await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => b.SaveChangesAsync());
    }
    [Theory]
    [InlineData(null)]
    [InlineData("abc,1,-2,2147483648,1,2")]
    [InlineData("1,2,3,4,5,6,7")]
    public void CookieParserNeverThrowsAndBoundsIds(string? text) { var ids = RecentCarIds.Parse(text); Assert.True(ids.Count <= 5); Assert.All(ids, x => Assert.True(x > 0)); Assert.Equal(ids.Count, ids.Distinct().Count()); }
    [Fact] public void ImageMagicRejectsExecutableAndRecognizesPng() { Assert.Null(ImageUpload.DetectExtension("<script>bad"u8)); Assert.Equal(".png", ImageUpload.DetectExtension(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })); Assert.False(ImageUpload.IsAllowedUrl("javascript:alert(1)")); Assert.False(ImageUpload.IsAllowedUrl("/images/cars/../../appsettings.json")); }
    [Fact]
    public async Task PublicQueryExcludesHiddenSoldAndInactiveAndFiltersBody()
    {
        using var s = new TestStore(); await s.Seed(); var cars = await s.Db.Cars.ToListAsync(); cars[0].Status = CarStatus.Available; cars[1].Status = CarStatus.Sold; cars[2].Status = CarStatus.Available; cars[2].IsPublished = false; await s.Db.SaveChangesAsync();
        var list = await new CarQuery(s.Db).ListAsync(new() { BodyType = "SUV", MinPrice = 700000000, MaxPrice = 900000000, YearFrom = 2023, YearTo = 2025, Sort = "price-asc" }); Assert.Single(list.Cars);
        var invalid = new CarFilter { MinPrice = 900000000, MaxPrice = 700000000 }; await Assert.ThrowsAsync<BusinessException>(() => new CarQuery(s.Db).ListAsync(invalid));
    }

    [Fact]
    public async Task ApiDoesNotExposeAnotherCustomersRequest()
    {
        using var store = new TestStore(); await store.Seed(); var customer = await store.Db.Customers.FindAsync(1); customer!.UserId = "staff"; await store.Db.SaveChangesAsync(); await new PurchaseService(store.Db).ImportAsync(1, 1, 600000000, "staff"); var id = await new SaleService(store.Db).RequestAsync(1, 1, "Private");
        var api = new Autora.Controllers.Api.PurchaseRequestsController(store.Db, new SaleService(store.Db));
        var context = new Microsoft.AspNetCore.Http.DefaultHttpContext(); context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "other") }, "Test")); api.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = context }; Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(await api.Get(id));
        context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "staff") }, "Test")); Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(await api.Get(id));
    }
}
