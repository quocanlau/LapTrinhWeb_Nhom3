using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public static class VersionGuard
{
    public static void Apply<T>(DbContext db, T entity, string? version) where T : class
    {
        if (string.IsNullOrWhiteSpace(version)) throw new BusinessException("Thiếu phiên bản. Hãy tải lại dữ liệu.");
        try { db.Entry(entity).Property("RowVersion").OriginalValue = Convert.FromBase64String(version); }
        catch (FormatException) { throw new BusinessException("Phiên bản không hợp lệ."); }
    }
}
