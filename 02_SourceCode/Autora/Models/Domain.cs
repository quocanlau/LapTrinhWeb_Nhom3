using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Autora.Models;

public enum CarStatus : byte
{
    [Display(Name="Chờ nhập")] AwaitingImport = 0,
    [Display(Name="Sẵn sàng")] Available = 1,
    [Display(Name="Đang giữ chờ cọc")] Reserved = 2,
    [Display(Name="Đã cọc")] Deposited = 3,
    [Display(Name="Đã bán")] Sold = 4
}
public enum CarCondition : byte { [Display(Name="Xe mới")] New = 0, [Display(Name="Xe đã sử dụng")] Used = 1 }
public enum PurchaseStatus : byte { Draft = 0, Confirmed = 1 }
public enum RequestStatus : byte { [Display(Name="Mới")] New = 0, [Display(Name="Đã chuyển giao dịch")] Converted = 1, [Display(Name="Đã đóng")] Closed = 2 }
public enum SaleStatus : byte { [Display(Name="Nháp")] Draft = 0, [Display(Name="Đang giữ chờ cọc")] Reserved = 1, [Display(Name="Đã cọc")] Deposited = 2, [Display(Name="Đã bàn giao")] Delivered = 3, [Display(Name="Đã hủy")] Cancelled = 4 }
public enum PaymentType : byte { Receipt = 0, Refund = 1 }
public static class Roles { public const string Customer="Customer", Employee="Employee", Admin="Admin"; public const string Staff=Admin+","+Employee; }
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    public bool IsActive { get; set; } = true;
}
public class Customer
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Required, StringLength(20)] public string Phone { get; set; } = "";
    [EmailAddress, StringLength(256)] public string? Email { get; set; }
    [StringLength(300)] public string? Address { get; set; }
}
public class Supplier
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [Required, StringLength(30)] public string SupplierType { get; set; } = "Cá nhân";
    [StringLength(20)] public string? Phone { get; set; }
    [StringLength(300)] public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
public class Manufacturer
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [StringLength(100)] public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
}
public class CarModel
{
    public int Id { get; set; }
    public int ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; } = null!;
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, StringLength(30)] public string BodyType { get; set; } = "Sedan";
    public bool IsActive { get; set; } = true;
}
public class Car
{
    public int Id { get; set; }
    public int CarModelId { get; set; }
    public CarModel CarModel { get; set; } = null!;
    [Required, RegularExpression("^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage="VIN phải có 17 ký tự hợp lệ, không gồm I, O, Q.")]
    public string VIN { get; set; } = "";
    [Range(1900,2100)] public short ManufactureYear { get; set; }
    [Required, StringLength(50)] public string Color { get; set; } = "";
    public CarCondition Condition { get; set; }
    [Range(0,int.MaxValue)] public int Mileage { get; set; }
    [Required, StringLength(30)] public string Fuel { get; set; } = "Xăng";
    [Required, StringLength(30)] public string Transmission { get; set; } = "Tự động";
    [Range(typeof(decimal),"1","9999999999999999")] public decimal ListedPrice { get; set; }
    public CarStatus Status { get; set; } = CarStatus.AwaitingImport;
    public bool IsPublished { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Timestamp] public byte[] RowVersion { get; set; } = [];
    public ICollection<CarImage> Images { get; set; } = new List<CarImage>();
}
public class CarImage
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    [Required, StringLength(1000)] public string ImagePath { get; set; } = "";
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
public class PurchaseReceipt
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public string EmployeeId { get; set; } = "";
    public ApplicationUser Employee { get; set; } = null!;
    public DateTimeOffset PurchasedAt { get; set; } = DateTimeOffset.UtcNow;
    public PurchaseStatus Status { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = [];
    public ICollection<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
}
public class PurchaseDetail
{
    public int Id { get; set; }
    public int PurchaseReceiptId { get; set; }
    public PurchaseReceipt PurchaseReceipt { get; set; } = null!;
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    public decimal PurchasePrice { get; set; }
}
public class DepositRequest
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    [StringLength(1000)] public string? Note { get; set; }
    public RequestStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Timestamp] public byte[] RowVersion { get; set; } = [];
}
public class SaleTransaction
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    public int? DepositRequestId { get; set; }
    public DepositRequest? DepositRequest { get; set; }
    public string EmployeeId { get; set; } = "";
    public ApplicationUser Employee { get; set; } = null!;
    public decimal AgreedPrice { get; set; }
    public decimal RequiredDeposit { get; set; }
    public SaleStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ConfirmedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Timestamp] public byte[] RowVersion { get; set; } = [];
    public ICollection<PaymentEntry> Payments { get; set; } = new List<PaymentEntry>();
}
public class PaymentEntry
{
    public int Id { get; set; }
    public int SaleTransactionId { get; set; }
    public SaleTransaction SaleTransaction { get; set; } = null!;
    public string CreatedById { get; set; } = "";
    public ApplicationUser CreatedBy { get; set; } = null!;
    public PaymentType EntryType { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(50)] public string PaymentMethod { get; set; } = "Tiền mặt";
    [StringLength(500)] public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public class Delivery
{
    public int Id { get; set; }
    public int SaleTransactionId { get; set; }
    public SaleTransaction SaleTransaction { get; set; } = null!;
    public DateTimeOffset DeliveredAt { get; set; } = DateTimeOffset.UtcNow;
    [Required, StringLength(150)] public string Recipient { get; set; } = "";
    [StringLength(500)] public string? Note { get; set; }
}
