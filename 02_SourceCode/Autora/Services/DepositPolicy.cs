using Autora.Models;
namespace Autora.Services;
public static class DepositPolicy
{
    public const decimal Rate=0.02m;
    public static decimal Calculate(decimal price)
    {
        if(price<=0) throw new BusinessException("Giá bán phải lớn hơn 0.");
        var amount=decimal.Round(price*Rate,0,MidpointRounding.AwayFromZero);
        if(amount<=0) throw new BusinessException("Giá bán quá nhỏ để tính tiền cọc.");
        return amount;
    }
    public static void RequireAvailable(Car car)
    {
        if(car.Status!=CarStatus.Available || !car.IsPublished)
            throw new BusinessException("Xe đã được giữ, đã cọc, đã bán hoặc chưa được mở bán. Không nhận yêu cầu cọc mới.");
    }
    public static decimal NetPaid(SaleTransaction sale) => sale.Payments.Sum(p=>p.EntryType==PaymentType.Receipt?p.Amount:-p.Amount);
    public static void ValidatePayment(SaleTransaction sale, PaymentType type, decimal amount)
    {
        if(sale.Status is not (SaleStatus.Reserved or SaleStatus.Deposited)) throw new BusinessException("Giao dịch không còn nhận thanh toán.");
        if(amount<=0 || decimal.Round(amount,0)!=amount) throw new BusinessException("Số tiền phải là số nguyên VND lớn hơn 0.");
        var paid=NetPaid(sale);
        if(type==PaymentType.Receipt && paid+amount>sale.AgreedPrice) throw new BusinessException("Khoản thu vượt số tiền còn phải thanh toán.");
        if(type==PaymentType.Refund && amount>paid) throw new BusinessException("Khoản hoàn vượt số tiền đã nhận.");
    }
}
public class BusinessException(string message) : Exception(message);
