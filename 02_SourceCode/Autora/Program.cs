using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Tự động tìm SQL Server đang chạy thực tế trên máy (SQLEXPRESS, LocalDB, localhost, .)
string ResolveConnectionString(IConfiguration config)
{
    var configured = config.GetConnectionString("DefaultConnection");
    var candidates = new List<string>();
    if (!string.IsNullOrWhiteSpace(configured)) candidates.Add(configured);
    candidates.Add("Server=.\\SQLEXPRESS;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=2");
    candidates.Add("Server=(localdb)\\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=2");
    candidates.Add("Server=localhost;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=2");
    candidates.Add("Server=.;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=2");

    foreach (var cs in candidates)
    {
        try
        {
            using var conn = new SqlConnection(cs);
            conn.Open();
            // Kết nối được instance này -> dùng luôn connection string này
            return cs;
        }
        catch
        {
            // Thử candidate tiếp theo
        }
    }
    return configured ?? "Server=(localdb)\\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
}

var activeConnectionString = ResolveConnectionString(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(activeConnectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 8;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(o =>
{
    o.ValidationInterval = TimeSpan.Zero;
});

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/AccessDenied";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = c =>
    {
        if (c.Request.Path.StartsWithSegments("/api")) { c.Response.StatusCode = 401; return Task.CompletedTask; }
        c.Response.Redirect(c.RedirectUri);
        return Task.CompletedTask;
    };
    o.Events.OnRedirectToAccessDenied = c =>
    {
        if (c.Request.Path.StartsWithSegments("/api")) { c.Response.StatusCode = 403; return Task.CompletedTask; }
        c.Response.Redirect(c.RedirectUri);
        return Task.CompletedTask;
    };
    o.Events.OnValidatePrincipal = async c =>
    {
        await SecurityStampValidator.ValidatePrincipalAsync(c);
        if (c.ShouldRenew && c.Principal?.Identity?.IsAuthenticated == true)
        {
            var manager = c.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await manager.GetUserAsync(c.Principal);
            if (user is null || !user.IsActive)
            {
                c.RejectPrincipal();
                await c.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }
        }
    };
});

builder.Services.AddResponseCompression(o => o.EnableForHttps = true);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddRateLimiter(o => { o.RejectionStatusCode = 429; o.AddPolicy("contact", ctx => RateLimitPartition.GetFixedWindowLimiter(ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 })); });
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");

   
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(o =>
{
    o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    o.Filters.Add<DatabaseErrorFilter>();
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "vi-VN", "vi", "en-US", "en" };
    options.SetDefaultCulture("vi-VN")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

                     
builder.Services.AddScoped<AdminNotificationService>();
builder.Services.AddScoped<CarQuery>();
builder.Services.AddScoped<ShowroomService>();
builder.Services.AddScoped<SaleService>();
builder.Services.AddScoped<PurchaseService>();

var app = builder.Build();

if (args.Contains("--seed"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsSqlServer())
    {
        await db.Database.MigrateAsync();
    }
    await DbSeeder.SeedAsync(scope.ServiceProvider, builder.Configuration, forceReset: args.Contains("--reset"));
    return;
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsSqlServer())
    {
        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.UseRouting();
app.UseRateLimiter();
app.UseRequestLocalization();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
public partial class Program { }
