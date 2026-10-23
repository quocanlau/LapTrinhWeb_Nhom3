using System.ComponentModel.DataAnnotations;
using Autora.Models;
namespace Autora.ViewModels;
public class ProfileForm
{
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Required, Phone, StringLength(20)] public string Phone { get; set; } = "";
    [StringLength(300)] public string? Address { get; set; }
}
public class PasswordForm
{
    [Required, DataType(DataType.Password)] public string CurrentPassword { get; set; } = "";
    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)] public string NewPassword { get; set; } = "";
    [Compare(nameof(NewPassword)), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = "";
}
public class CustomerForm
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Required, Phone, StringLength(20)] public string Phone { get; set; } = "";
    [EmailAddress, StringLength(256)] public string? Email { get; set; }
    [StringLength(300)] public string? Address { get; set; }
}
public class SupplierForm
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [Required, RegularExpression("Cá nhân|Tổ chức")] public string SupplierType { get; set; } = "Cá nhân";
    [Phone, StringLength(20)] public string? Phone { get; set; }
    [StringLength(300)] public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
public class ModelForm
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int ManufacturerId { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, StringLength(30)] public string BodyType { get; set; } = "Sedan";
    public bool IsActive { get; set; } = true;
}
public class PurchaseLineForm
{
    [Range(1, int.MaxValue)] public int CarId { get; set; }
    [Range(typeof(decimal), "1", "9999999999999999")] public decimal PurchasePrice { get; set; }
}
public class PurchaseDraftForm
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int SupplierId { get; set; }
    public string? Version { get; set; }
    public List<PurchaseLineForm> Lines { get; set; } = [];
}
public class SaleDraftForm
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    [Range(1, int.MaxValue)] public int CarId { get; set; }
    [Range(typeof(decimal), "1", "9999999999999999")] public decimal AgreedPrice { get; set; }
    public string? Version { get; set; }
}
public class PageResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int Pages { get; set; }
    public int Total { get; set; }
    public string? Keyword { get; set; }
}
public class DashboardModel
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int Stock { get; set; }
    public int Sold { get; set; }
    public decimal Revenue { get; set; }
    public decimal NetCash { get; set; }
    public decimal Receivable { get; set; }
    public decimal PurchaseValue { get; set; }
    public decimal GrossProfit { get; set; }
}
