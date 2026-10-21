using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Autora.Data;
using Autora.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace Autora.Tests;
public class DemoFactory : WebApplicationFactory<Program>
{
    public TestStore Store { get; } = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder) { builder.UseEnvironment("Development"); builder.ConfigureServices(s => s.AddScoped<AppDbContext>(_ => Store.NewContext())); }
    public async Task Initialize()
    {
        await Store.Seed(); using var scope = Services.CreateScope(); var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(); var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        foreach (var role in new[] { Roles.Admin, Roles.Employee, Roles.Customer }) Assert.True((await roles.CreateAsync(new IdentityRole(role))).Succeeded);
        var staff = await manager.FindByIdAsync("staff"); staff!.Email = "staff@test.local"; Assert.True((await manager.UpdateAsync(staff)).Succeeded); Assert.True((await manager.AddPasswordAsync(staff, "Staff@12345")).Succeeded); Assert.True((await manager.AddToRoleAsync(staff, Roles.Admin)).Succeeded);
        var u = new ApplicationUser { Id = "buyer", UserName = "buyer@test.local", Email = "buyer@test.local", FullName = "Buyer", PhoneNumber = "0900000000" }; Assert.True((await manager.CreateAsync(u, "Buyer@12345")).Succeeded); Assert.True((await manager.AddToRoleAsync(u, Roles.Customer)).Succeeded); var c = await db.Customers.FindAsync(1); c!.UserId = "buyer"; await db.SaveChangesAsync();
    }
}
public class HttpWorkflowTests
{
    private static async Task<string> Token(HttpClient c, string page = "/Account/Login")
    {
        var r = await c.GetAsync(page); Assert.Equal(HttpStatusCode.OK, r.StatusCode); var html = await r.Content.ReadAsStringAsync(); var m = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\""); Assert.True(m.Success); return WebUtility.HtmlDecode(m.Groups[1].Value);
    }
    private static async Task<HttpResponseMessage> Post(HttpClient c, string url, Dictionary<string, string> data, string page = "/Account/Profile") { data["__RequestVerificationToken"] = await Token(c, page); return await c.PostAsync(url, new FormUrlEncodedContent(data)); }
    private static async Task Login(HttpClient c, string email, string password) { Assert.Equal(HttpStatusCode.Redirect, (await Post(c, "/Account/Login", new() { ["Email"] = email, ["Password"] = password }, "/Account/Login")).StatusCode); }
    [Fact]
    public async Task AdminAndCustomerCompleteWorkflowThroughHttp()
    {
        await using var f = new DemoFactory(); await f.Initialize(); using var admin = f.CreateClient(new() { AllowAutoRedirect = false }); using var buyer = f.CreateClient(new() { AllowAutoRedirect = false }); await Login(admin, "staff@test.local", "Staff@12345");
        foreach (var path in new[] { "/Admin/Home", "/Admin/Car", "/Admin/Purchase", "/Admin/Customer", "/Admin/Supplier", "/Admin/User", "/Admin/Setting", "/Admin/Order", "/Admin/Notification", "/Admin/Contact" }) { var result = await admin.GetAsync(path); Assert.True(result.StatusCode == HttpStatusCode.OK, path + " " + await result.Content.ReadAsStringAsync()); }
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Setting/SaveShowroom", new() { ["Name"] = "Demo Updated", ["Hotline"] = "0900000000", ["Email"] = "demo@test.local", ["Address"] = "Hanoi" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Purchase/Save", new() { ["SupplierId"] = "1", ["Lines[0].CarId"] = "1", ["Lines[0].PurchasePrice"] = "600000000", ["Lines[1].CarId"] = "2", ["Lines[1].PurchasePrice"] = "650000000" })).StatusCode);
        int pid; string version; using (var db = f.Store.NewContext()) { var p = await db.PurchaseReceipts.SingleAsync(); pid = p.Id; version = Convert.ToBase64String(p.RowVersion); }
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Purchase/Confirm", new() { ["id"] = pid.ToString(), ["version"] = version })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/Car?BodyType=SUV&MinPrice=700000000&YearFrom=2023")).StatusCode);
        var cookieRequest = new HttpRequestMessage(HttpMethod.Get, "/Car/Details/1"); cookieRequest.Headers.Add("Cookie", "Autora_RecentlyViewedCars=abc,2147483648,-1,2"); Assert.Equal(HttpStatusCode.OK, (await admin.SendAsync(cookieRequest)).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Order/SaveDraft", new() { ["CustomerId"] = "1", ["CarId"] = "1", ["AgreedPrice"] = "800000000" })).StatusCode);
        int sid; using (var db = f.Store.NewContext()) { var s = await db.SaleTransactions.SingleAsync(); sid = s.Id; version = Convert.ToBase64String(s.RowVersion); }
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Order/ConfirmDraft", new() { ["id"] = sid.ToString(), ["version"] = version })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Order/Pay", new() { ["SaleId"] = sid.ToString(), ["EntryType"] = "0", ["Amount"] = "16000000", ["PaymentMethod"] = "Cash" })).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(admin, "/Admin/Order/Deliver", new() { ["SaleId"] = sid.ToString(), ["Recipient"] = "Buyer" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Order/Pay", new() { ["SaleId"] = sid.ToString(), ["EntryType"] = "0", ["Amount"] = "784000000", ["PaymentMethod"] = "Cash" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/Order/Deliver", new() { ["SaleId"] = sid.ToString(), ["Recipient"] = "Buyer" })).StatusCode);
        await Login(buyer, "buyer@test.local", "Buyer@12345"); Assert.Equal(HttpStatusCode.Redirect, (await Post(buyer, "/Compare/Add", new() { ["carId"] = "2" })).StatusCode); Assert.Contains("Model", await buyer.GetStringAsync("/Compare"));
        Assert.Equal(HttpStatusCode.Redirect, (await Post(buyer, "/Account/Profile", new() { ["FullName"] = "O'Connor", ["Phone"] = "0900000001", ["Address"] = "Test address" })).StatusCode);
        var token = await Token(buyer, "/Account/Profile"); using var message = new HttpRequestMessage(HttpMethod.Post, "/api/purchase-requests") { Content = new StringContent("{\"carId\":2,\"note\":\"Please contact\"}", Encoding.UTF8, "application/json") }; message.Headers.Add("X-CSRF-TOKEN", token); var api = await buyer.SendAsync(message); Assert.Equal(HttpStatusCode.Created, api.StatusCode); var doc = JsonDocument.Parse(await api.Content.ReadAsStringAsync()); Assert.Equal(HttpStatusCode.OK, (await buyer.GetAsync("/api/purchase-requests/" + doc.RootElement.GetProperty("id").GetInt32())).StatusCode);
        using (var db = f.Store.NewContext()) { Assert.Equal("Demo Updated", (await db.ShowroomSettings.SingleAsync()).Name); Assert.Equal(CarStatus.Sold, (await db.Cars.FindAsync(1))!.Status); Assert.Single(await db.Deliveries.ToListAsync()); }
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/Admin/Home")).StatusCode); Assert.Equal(HttpStatusCode.Redirect, (await buyer.GetAsync("/Admin/User")).StatusCode); Assert.Equal(HttpStatusCode.BadRequest, (await buyer.PostAsync("/Cart/Add", new FormUrlEncodedContent(new Dictionary<string, string> { { "carId", "2" } }))).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(admin, "/Admin/User/Create", new() { ["FullName"] = "Second Admin", ["Email"] = "second@test.local", ["Phone"] = "0900000002", ["Password"] = "Second@12345", ["Role"] = "Admin" })).StatusCode);
        using var second = f.CreateClient(new() { AllowAutoRedirect = false }); await Login(second, "second@test.local", "Second@12345");
        Assert.Equal(HttpStatusCode.Redirect, (await Post(second, "/Admin/User/Delete", new() { ["id"] = "staff" })).StatusCode);
        using (var db = f.Store.NewContext()) { Assert.False((await db.Users.FindAsync("staff"))!.IsActive); Assert.All(await db.PaymentEntries.ToListAsync(), p => Assert.Equal("staff", p.CreatedById)); Assert.Equal("staff", (await db.SaleTransactions.SingleAsync()).EmployeeId); }
        Assert.Equal(HttpStatusCode.Redirect, (await admin.GetAsync("/Admin/Notification/Feed")).StatusCode);

    }
}
