using System.ComponentModel.DataAnnotations;
namespace Autora.Models;
public class ShowroomSetting
{
    public int Id { get; set; } = 1;
    [Required, StringLength(150)] public string Name { get; set; } = "AUTORA MOTORS";
    [Required, StringLength(50)] public string Hotline { get; set; } = "1900 8888";
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = "contact@autora.vn";
    [Required, StringLength(300)] public string Address { get; set; } = "Hà Nội";
    [StringLength(100)] public string? OpeningHours { get; set; } = "08:00–20:00";
    [StringLength(1000)] public string? Policy { get; set; } = "Cọc 2% giá bán chốt. Hoàn đủ tiền đã thu trước khi hủy.";
    [Timestamp] public byte[] RowVersion { get; set; } = [];
}
