using System.Linq.Expressions;
using Autora.Models;
namespace Autora.Services;
public static class CarVisibility
{
    public static readonly Expression<Func<Car, bool>> Public = c => c.IsPublished && c.Status != CarStatus.AwaitingImport && c.Status != CarStatus.Sold && c.CarModel.IsActive && c.CarModel.Manufacturer.IsActive;
}
public static class RecentCarIds
{
    public static List<int> Parse(string? value) => (value ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.TryParse(x, out var n) ? n : 0).Where(x => x > 0).Distinct().Take(5).ToList();
}
