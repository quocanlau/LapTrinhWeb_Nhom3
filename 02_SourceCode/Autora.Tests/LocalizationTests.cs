using System.Globalization;
using Autora.Models;
using Autora.Services;
using Xunit;

namespace Autora.Tests;

public class LocalizationTests
{
    [Fact]
    public void CarTranslationHelper_TranslatesToEnglish_WhenCultureIsEnglish()
    {
        var prevCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            Assert.True(CarTranslationHelper.IsEnglish);
            Assert.Equal("Gasoline", CarTranslationHelper.TranslateFuel("Xăng"));
            Assert.Equal("Diesel", CarTranslationHelper.TranslateFuel("Dầu"));
            Assert.Equal("Electric", CarTranslationHelper.TranslateFuel("Điện"));
            Assert.Equal("Hybrid", CarTranslationHelper.TranslateFuel("Hybrid"));

            Assert.Equal("Automatic", CarTranslationHelper.TranslateTransmission("Tự động"));
            Assert.Equal("Manual", CarTranslationHelper.TranslateTransmission("Số sàn"));

            Assert.Equal("Brand New", CarTranslationHelper.TranslateCondition(CarCondition.New));
            Assert.Equal("Pre-owned", CarTranslationHelper.TranslateCondition(CarCondition.Used));

            Assert.Equal("Available", CarTranslationHelper.TranslateStatus(CarStatus.Available));
            Assert.Equal("Deposited", CarTranslationHelper.TranslateStatus(CarStatus.Deposited));

            Assert.Equal("White", CarTranslationHelper.TranslateColor("Trắng"));
            Assert.Equal("Black", CarTranslationHelper.TranslateColor("Đen"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = prevCulture;
        }
    }

    [Fact]
    public void CarTranslationHelper_KeepsVietnamese_WhenCultureIsVietnamese()
    {
        var prevCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("vi-VN");

            Assert.False(CarTranslationHelper.IsEnglish);
            Assert.Equal("Xăng", CarTranslationHelper.TranslateFuel("Xăng"));
            Assert.Equal("Tự động", CarTranslationHelper.TranslateTransmission("Tự động"));
            Assert.Equal("Xe mới", CarTranslationHelper.TranslateCondition(CarCondition.New));
            Assert.Equal("Sẵn sàng", CarTranslationHelper.TranslateStatus(CarStatus.Available));
            Assert.Equal("Trắng", CarTranslationHelper.TranslateColor("Trắng"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = prevCulture;
        }
    }
}
