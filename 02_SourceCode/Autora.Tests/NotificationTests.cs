using System.Reflection;
using Autora.Areas.Admin.Controllers;
using Autora.Data;
using Autora.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Autora.Tests;
public class NotificationTests
{
    [Fact] public void NotificationAccessRequiresStaffRole()
    {
        var auth=typeof(NotificationController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(auth);Assert.Equal(Roles.Staff,auth.Roles);
        Assert.DoesNotContain(Roles.Customer,auth.Roles!);
    }
    [Theory]
    [InlineData("Read")][InlineData("Open")][InlineData("ReadAll")][InlineData("ReadAllPage")]
    public void MarkingReadRequiresPostAndCsrf(string name)
    {
        var method=typeof(NotificationController).GetMethod(name)!;
        Assert.NotNull(method.GetCustomAttribute<HttpPostAttribute>());
        Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
        // User id is not a client-controlled action parameter.
        Assert.DoesNotContain(method.GetParameters(),p=>p.Name=="userId");
    }
    [Fact] public void ReadStateIsUniquePerUserAndEvent()
    {
        using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer("Server=localhost;Database=MetadataOnly;Trusted_Connection=True;TrustServerCertificate=True").Options);
        var entity=db.Model.FindEntityType(typeof(AdminNotificationRead))!;
        Assert.Contains(entity.GetIndexes(),i=>i.IsUnique && i.Properties.Select(p=>p.Name).SequenceEqual(new[]{"UserId","EventKey"}));
        Assert.Equal(DeleteBehavior.Cascade,entity.GetForeignKeys().Single().DeleteBehavior);
        Assert.Equal(100,entity.FindProperty("EventKey")!.GetMaxLength());
    }
}
