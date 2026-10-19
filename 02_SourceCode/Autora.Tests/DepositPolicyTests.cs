using Autora.Models;
using Autora.Services;
using Autora.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Autora.Tests;
public class DepositPolicyTests
{
    [Theory]
    [InlineData(2799000000L,55980000L)]
    [InlineData(800000000L,16000000L)]
    [InlineData(125L,3L)]
    public void DepositIsTwoPercentRoundedToWholeVnd(long price,long expected)=>Assert.Equal((decimal)expected,DepositPolicy.Calculate(price));
    [Theory][InlineData(0)][InlineData(-1)][InlineData(1)]
    public void InvalidOrTooSmallPricesAreRejected(int price)=>Assert.Throws<BusinessException>(()=>DepositPolicy.Calculate(price));
    [Theory][InlineData(CarStatus.AwaitingImport)][InlineData(CarStatus.Reserved)][InlineData(CarStatus.Deposited)][InlineData(CarStatus.Sold)]
    public void NonAvailableCarsCannotAcceptNewRequests(CarStatus status)=>Assert.Throws<BusinessException>(()=>DepositPolicy.RequireAvailable(new Car{IsPublished=true,Status=status}));
    [Fact]public void UnpublishedCarCannotAcceptRequest()=>Assert.Throws<BusinessException>(()=>DepositPolicy.RequireAvailable(new Car{Status=CarStatus.Available}));
    [Fact]public void AvailablePublishedCarCanAcceptRequest()=>DepositPolicy.RequireAvailable(new Car{Status=CarStatus.Available,IsPublished=true});
    private static SaleTransaction Sale()=>new(){Status=SaleStatus.Deposited,AgreedPrice=800000000m,RequiredDeposit=16000000m,Payments=[new PaymentEntry{EntryType=PaymentType.Receipt,Amount=16000000m}]};
    [Fact]public void ExistingBuyerCanPayRemainingBalance()=>DepositPolicy.ValidatePayment(Sale(),PaymentType.Receipt,784000000m);
    [Fact]public void OverpaymentRejected()=>Assert.Throws<BusinessException>(()=>DepositPolicy.ValidatePayment(Sale(),PaymentType.Receipt,784000001m));
    [Fact]public void RefundCannotExceedNetPaid()=>Assert.Throws<BusinessException>(()=>DepositPolicy.ValidatePayment(Sale(),PaymentType.Refund,16000001m));
    [Fact]public void FractionalVndRejected()=>Assert.Throws<BusinessException>(()=>DepositPolicy.ValidatePayment(Sale(),PaymentType.Receipt,1.5m));
    [Fact]public void CancelledSaleCannotReceivePayment(){var sale=Sale();sale.Status=SaleStatus.Cancelled;Assert.Throws<BusinessException>(()=>DepositPolicy.ValidatePayment(sale,PaymentType.Receipt,1m));}
    [Fact]public void RefundsReduceNetPaid(){var sale=Sale();sale.Payments.Add(new PaymentEntry{EntryType=PaymentType.Refund,Amount=1000000m});Assert.Equal(15000000m,DepositPolicy.NetPaid(sale));}
    [Fact]public void SqlServerModelBuildsWithConcurrencyAndActiveSaleConstraint()
    {
        using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer("Server=localhost;Database=AutoraTest;Trusted_Connection=True;TrustServerCertificate=True").Options);
        var entity=db.Model.FindEntityType(typeof(SaleTransaction))!;
        Assert.True(entity.FindProperty(nameof(SaleTransaction.RowVersion))!.IsConcurrencyToken);
        Assert.Contains(entity.GetIndexes(),i=>i.IsUnique && i.GetFilter()=="[Status] IN (1, 2, 3)");
        var sql=db.Database.GenerateCreateScript();Assert.Contains("CK_Sale_Deposit",sql);Assert.Contains("rowversion",sql);
    }
}
