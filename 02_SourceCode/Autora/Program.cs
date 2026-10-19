using Microsoft.AspNetCore.Authentication;
using Autora.Data;
using Autora.Models;
using Autora.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o => {
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 8;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(o => {
    o.ValidationInterval = TimeSpan.FromMinutes(10);
});

builder.Services.ConfigureApplicationCookie(o => {
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/AccessDenied";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = c => {
        if (c.Request.Path.StartsWithSegments("/api")) { c.Response.StatusCode = 401; return Task.CompletedTask; }
        c.Response.Redirect(c.RedirectUri);
        return Task.CompletedTask;
    };
    o.Events.OnRedirectToAccessDenied = c => {
        if (c.Request.Path.StartsWithSegments("/api")) { c.Response.StatusCode = 403; return Task.CompletedTask; }
        c.Response.Redirect(c.RedirectUri);
        return Task.CompletedTask;
    };
    o.Events.OnValidatePrincipal = async c => {
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
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");

// 
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(o => {
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

// Services nghiệp vụ
builder.Services.AddScoped<AdminNotificationService>();
builder.Services.AddScoped<CarQuery>();
builder.Services.AddScoped<SaleService>();
builder.Services.AddScoped<PurchaseService>();

var app = builder.Build();

if (args.Contains("--seed"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
    return;
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
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.UseRouting();
app.UseRequestLocalization();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();