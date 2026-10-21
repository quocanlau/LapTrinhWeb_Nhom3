using Autora.Data;
using Autora.Models;
using Microsoft.EntityFrameworkCore;
namespace Autora.Services;
public class ShowroomService(AppDbContext db)
{
    private ShowroomSetting? cached;
    public async Task<ShowroomSetting> GetAsync() => cached ??= await db.ShowroomSettings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == 1) ?? new ShowroomSetting();
}
