namespace Autora.ViewModels;
public record AdminNotificationItem(string Key, string Title, string Message, string Url, DateTimeOffset CreatedAt, string Kind, bool IsRead = false);
public class AdminNotificationPage
{
    public List<AdminNotificationItem> Items { get; set; } = [];
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages => Math.Max(1,(int)Math.Ceiling(TotalCount/20d));
    public bool UnreadOnly { get; set; }
}
