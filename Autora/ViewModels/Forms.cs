using System.ComponentModel.DataAnnotations;
using Autora.Models;
namespace Autora.ViewModels;
public class LoginForm
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
public class RegisterForm
{
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Required, Phone, StringLength(20)] public string Phone { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)] public string Password { get; set; } = "";
    [Required, Compare(nameof(Password)), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = "";
}
public class CarFilter
{
    public int? ManufacturerId { get; set; }
    public int? BrandId { get; set; }
    public int? CarModelId { get; set; }
    public string? BodyType { get; set; }
    public string? Transmission { get; set; }
    public decimal? MinPrice { get; set; }
    public short? YearFrom { get; set; }
    public short? YearTo { get; set; }
    [StringLength(100)] public string? Keyword { get; set; }
    public string? Fuel { get; set; }
    public CarCondition? Condition { get; set; }
    public decimal? MaxPrice { get; set; }
    public string Sort { get; set; } = "latest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
public class CarCatalog
{
    public CarFilter Filter { get; set; } = new();
    public List<Car> Cars { get; set; } = [];
    public List<Manufacturer> Manufacturers { get; set; } = [];
    public List<CarModel> CarModels { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Filter.PageSize);
}
public class RequestForm
{
    [Range(1, int.MaxValue)] public int CarId { get; set; }
    [StringLength(1000)] public string? Note { get; set; }
}
public class ReserveForm
{
    [Range(1, int.MaxValue)] public int RequestId { get; set; }
    [Range(typeof(decimal), "1", "9999999999999999")] public decimal AgreedPrice { get; set; }
}
public class PayForm
{
    [Range(1, int.MaxValue)] public int SaleId { get; set; }
    [EnumDataType(typeof(PaymentType))] public PaymentType EntryType { get; set; }
    [Range(typeof(decimal), "1", "9999999999999999")] public decimal Amount { get; set; }
    [Required, StringLength(50)] public string PaymentMethod { get; set; } = "Tiền mặt";
    [StringLength(500)] public string? Note { get; set; }
}
public class DeliveryForm
{
    [Range(1, int.MaxValue)] public int SaleId { get; set; }
    [Required, StringLength(150)] public string Recipient { get; set; } = "";
    [StringLength(500)] public string? Note { get; set; }
}
public class ImportForm
{
    [Range(1, int.MaxValue)] public int CarId { get; set; }
    [Range(1, int.MaxValue)] public int SupplierId { get; set; }
    [Range(typeof(decimal), "1", "9999999999999999")] public decimal PurchasePrice { get; set; }
}
public class CarForm
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int CarModelId { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập số VIN.")]
    [RegularExpression("^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "VIN phải gồm đúng 17 ký tự chữ và số, không chứa I, O hoặc Q.")]
    public string VIN { get; set; } = "";
    [Range(1900, 2100)] public short ManufactureYear { get; set; } = 2024;
    [Required, StringLength(50)] public string Color { get; set; } = "";
    [EnumDataType(typeof(CarCondition))] public CarCondition Condition { get; set; }
    [Range(0, int.MaxValue)] public int Mileage { get; set; }
    [Required, StringLength(30)] public string Fuel { get; set; } = "Xăng";
    [Required, StringLength(30)] public string Transmission { get; set; } = "Tự động";
    [Range(typeof(decimal), "1", "9999999999999999")] public decimal ListedPrice { get; set; }
    public bool IsPublished { get; set; }
    [StringLength(1000)] public string? ImageUrl { get; set; }
    public string? Version { get; set; }
}
public class StaffOrders
{
    public int Page { get; set; } = 1;
    public int Pages { get; set; } = 1;
    public List<DepositRequest> Requests { get; set; } = [];
    public List<SaleTransaction> Sales { get; set; } = [];
}
public class CustomerOrders
{
    public List<DepositRequest> Requests { get; set; } = [];
    public List<SaleTransaction> Sales { get; set; } = [];
}
public class UserItemViewModel
{
    public string Id { get; set; } = "";
    public string Code { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public bool IsCurrentAdmin { get; set; } = false;
    public string Initials { get; set; } = "";
    public bool HasTransactions { get; set; } = false;
}
public class UserListViewModel
{
    public int Page { get; set; } = 1;
    public int Pages { get; set; } = 1;
    public int Total { get; set; }
    public string? Keyword { get; set; }
    public List<UserItemViewModel> Users { get; set; } = [];
    public int TotalUsers => Users.Count;
    public int ActiveUsers => Users.Count(u => u.IsActive);
    public int AdminUsers => Users.Count(u => u.Role == Roles.Admin);
    public int EmployeeUsers => Users.Count(u => u.Role == Roles.Employee);
}
public class UserCreateForm
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(150, ErrorMessage = "Họ và tên không quá 150 ký tự")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20)]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn vai trò")]
    public string Role { get; set; } = Roles.Employee;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu khởi tạo")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 ký tự trở lên")]
    public string Password { get; set; } = "";
}

