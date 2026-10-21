using System.ComponentModel.DataAnnotations;
namespace Autora.Models;
public class ContactInquiry
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Phone, StringLength(20)] public string? Phone { get; set; }
    [EmailAddress, StringLength(256)] public string? Email { get; set; }
    [StringLength(100)] public string? InterestedModel { get; set; }
    [StringLength(1000)] public string? Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
