using Autora.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Autora.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<AdminNotificationRead> AdminNotificationReads => Set<AdminNotificationRead>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarImage> CarImages => Set<CarImage>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseReceipt> PurchaseReceipts => Set<PurchaseReceipt>();
    public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
    public DbSet<DepositRequest> DepositRequests => Set<DepositRequest>();
    public DbSet<SaleTransaction> SaleTransactions => Set<SaleTransaction>();
    public DbSet<PaymentEntry> PaymentEntries => Set<PaymentEntry>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Manufacturer>().HasIndex(x=>x.Name).IsUnique();
        b.Entity<CarModel>().HasIndex(x=>new{x.ManufacturerId,x.Name}).IsUnique();
        b.Entity<Customer>().HasIndex(x=>x.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL");
        b.Entity<Customer>().HasOne(x=>x.User).WithOne().HasForeignKey<Customer>(x=>x.UserId);
        b.Entity<Car>().Property(x=>x.VIN).HasMaxLength(17).IsUnicode(false);
        b.Entity<Car>().HasIndex(x=>x.VIN).IsUnique();
        b.Entity<Car>().HasIndex(x=>new{x.IsPublished,x.Status,x.CarModelId});
        b.Entity<CarImage>().HasIndex(x=>x.CarId).IsUnique().HasFilter("[IsPrimary] = 1");
        b.Entity<PurchaseDetail>().HasIndex(x=>x.CarId).IsUnique();
        b.Entity<DepositRequest>().HasIndex(x=>new{x.CustomerId,x.CarId}).IsUnique().HasFilter("[Status] = 0");
        b.Entity<SaleTransaction>().HasIndex(x=>x.DepositRequestId).IsUnique().HasFilter("[DepositRequestId] IS NOT NULL");
        b.Entity<SaleTransaction>().HasIndex(x=>x.CarId).IsUnique().HasFilter("[Status] IN (1, 2, 3)");
        b.Entity<Delivery>().HasIndex(x=>x.SaleTransactionId).IsUnique();
        b.Entity<Car>().ToTable(t=>{
            t.HasCheckConstraint("CK_Car_Price", "[ListedPrice] > 0");
            t.HasCheckConstraint("CK_Car_Mileage", "[Mileage] >= 0");
            t.HasCheckConstraint("CK_Car_Status", "[Status] BETWEEN 0 AND 4");
            t.HasCheckConstraint("CK_Car_Condition", "[Condition] IN (0, 1)");
            t.HasCheckConstraint("CK_Car_Year", "[ManufactureYear] BETWEEN 1900 AND 2100");
        });
        b.Entity<PurchaseDetail>().ToTable(t=>t.HasCheckConstraint("CK_PurchaseDetail_Price", "[PurchasePrice] > 0"));
        b.Entity<PurchaseReceipt>().ToTable(t=>t.HasCheckConstraint("CK_Purchase_Status","[Status] IN (0, 1)"));
        b.Entity<DepositRequest>().ToTable(t=>t.HasCheckConstraint("CK_Request_Status","[Status] BETWEEN 0 AND 2"));
        b.Entity<SaleTransaction>().ToTable(t=>{
            t.HasCheckConstraint("CK_Sale_Price","[AgreedPrice] > 0");
            t.HasCheckConstraint("CK_Sale_Deposit","[RequiredDeposit] = ROUND([AgreedPrice] * 0.02, 0)");
            t.HasCheckConstraint("CK_Sale_Status","[Status] BETWEEN 0 AND 4");
        });
        b.Entity<PaymentEntry>().ToTable(t=>{
            t.HasCheckConstraint("CK_Payment_Amount","[Amount] > 0");
            t.HasCheckConstraint("CK_Payment_Type","[EntryType] IN (0, 1)");
        });
        foreach(var entity in b.Model.GetEntityTypes())
        {
            foreach(var prop in entity.GetProperties())
                if(prop.ClrType==typeof(decimal)) { prop.SetPrecision(18); prop.SetScale(2); }
            foreach(var fk in entity.GetForeignKeys()) fk.DeleteBehavior=DeleteBehavior.Restrict;
        }
        b.Entity<AdminNotificationRead>().HasIndex(x=>new{x.UserId,x.EventKey}).IsUnique();
        b.Entity<AdminNotificationRead>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
