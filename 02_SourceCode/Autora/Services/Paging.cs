using Autora.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public static class Paging
{
    public static async Task<PageResult<T>> PageAsync<T>(this IQueryable<T> query, int page, string? keyword = null)
    {
        var total = await query.CountAsync(); var pages = Math.Max(1, (int)Math.Ceiling(total / 20d)); page = Math.Clamp(page, 1, pages);
        return new() { Items = await query.Skip((page - 1) * 20).Take(20).ToListAsync(), Page = page, Pages = pages, Total = total, Keyword = keyword };
    }
}
