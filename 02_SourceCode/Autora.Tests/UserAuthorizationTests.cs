using System.Reflection;
using Autora.Areas.Admin.Controllers;
using Autora.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Autora.Tests;

public class UserAuthorizationTests
{
    [Fact]
    public void UserController_MustBeProtectedWithAdminRoleOnly()
    {
        var authAttr = typeof(UserController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authAttr);
        Assert.Equal(Roles.Admin, authAttr.Roles);
        Assert.DoesNotContain(Roles.Employee, authAttr.Roles);
        Assert.DoesNotContain(Roles.Customer, authAttr.Roles);
    }

    [Fact]
    public void UserController_MustBeInAdminArea()
    {
        var areaAttr = typeof(UserController).GetCustomAttribute<AreaAttribute>();
        Assert.NotNull(areaAttr);
        Assert.Equal("Admin", areaAttr.RouteValue);
    }

    [Fact]
    public void Employee_RoleDefinition_CannotAccessAdminExclusiveEndpoints()
    {
        // Verify Employee role does NOT match Roles.Admin
        Assert.NotEqual(Roles.Admin, Roles.Employee);
        Assert.Equal("Admin", Roles.Admin);
        Assert.Equal("Employee", Roles.Employee);
    }

    [Theory]
    [InlineData("Create")]
    [InlineData("ToggleStatus")]
    [InlineData("ResetPassword")]
    [InlineData("Delete")]
    public void UserController_StateModifyingActions_MustRequireHttpPostAndAntiCsrf(string actionName)
    {
        var method = typeof(UserController).GetMethod(actionName);
        Assert.NotNull(method);

        var httpPost = method.GetCustomAttribute<HttpPostAttribute>();
        var antiForgery = method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>();

        Assert.NotNull(httpPost);
        Assert.NotNull(antiForgery);
    }
}
