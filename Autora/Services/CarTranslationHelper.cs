using System.Globalization;
using Autora.Models;

namespace Autora.Services;

public static class CarTranslationHelper
{
    public static bool IsEnglish =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase);

    public static string TranslateFuel(string? fuel)
    {
        if (string.IsNullOrWhiteSpace(fuel)) return "";
        if (!IsEnglish) return fuel;
        return fuel.Trim().ToLowerInvariant() switch
        {
            "xăng" => "Gasoline",
            "dầu" => "Diesel",
            "điện" => "Electric",
            "hybrid" => "Hybrid",
            _ => fuel
        };
    }

    public static string TranslateTransmission(string? transmission)
    {
        if (string.IsNullOrWhiteSpace(transmission)) return "";
        if (!IsEnglish) return transmission;
        return transmission.Trim().ToLowerInvariant() switch
        {
            "tự động" or "số tự động" => "Automatic",
            "số sàn" or "sàn" => "Manual",
            _ => transmission
        };
    }

    public static string TranslateCondition(CarCondition condition)
    {
        if (!IsEnglish)
        {
            return condition == CarCondition.New ? "Xe mới" : "Xe đã sử dụng";
        }
        return condition == CarCondition.New ? "Brand New" : "Pre-owned";
    }

    public static string TranslateColor(string? color)
    {
        if (string.IsNullOrWhiteSpace(color)) return "";
        if (!IsEnglish) return color;
        return color.Trim().ToLowerInvariant() switch
        {
            "trắng" => "White",
            "đen" => "Black",
            "đỏ" => "Red",
            "xanh" or "xanh dương" => "Blue",
            "xanh lá" => "Green",
            "bạc" => "Silver",
            "xám" => "Grey",
            "vàng" or "vàng cát" => "Gold",
            "nâu" => "Brown",
            _ => color
        };
    }

    public static string TranslateStatus(CarStatus status)
    {
        if (!IsEnglish)
        {
            return status switch
            {
                CarStatus.AwaitingImport => "Chờ nhập",
                CarStatus.Available => "Sẵn sàng",
                CarStatus.Reserved => "Đang giữ chờ cọc",
                CarStatus.Deposited => "Đã cọc",
                CarStatus.Sold => "Đã bán",
                _ => status.ToString()
            };
        }
        return status switch
        {
            CarStatus.AwaitingImport => "Incoming",
            CarStatus.Available => "Available",
            CarStatus.Reserved => "Reserved",
            CarStatus.Deposited => "Deposited",
            CarStatus.Sold => "Sold",
            _ => status.ToString()
        };
    }

    public static string TranslateBodyType(string? bodyType)
    {
        if (string.IsNullOrWhiteSpace(bodyType)) return "";
        if (!IsEnglish) return bodyType;
        return bodyType.Trim().ToLowerInvariant() switch
        {
            "sedan" => "Sedan",
            "suv" => "SUV",
            "mpv" => "MPV",
            "hatchback" => "Hatchback",
            "crossover" => "Crossover",
            "bán tải" => "Pickup Truck",
            _ => bodyType
        };
    }
}
