using System.ComponentModel.DataAnnotations;
namespace Autora.Models;
public class AdminNotificationRead
{
    public long Id { get; set; }
    [Required, StringLength(450)] public string UserId { get; set; } = "";
    [Required, StringLength(100)] public string EventKey { get; set; } = "";
    public DateTimeOffset ReadAt { get; set; } = DateTimeOffset.UtcNow;
}
