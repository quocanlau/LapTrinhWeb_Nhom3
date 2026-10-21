namespace Autora.Services;
public static class ImageUpload
{
    public static string? DetectExtension(ReadOnlySpan<byte> b)
    {
        if (b.Length >= 8 && b[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return ".png";
        if (b.Length >= 3 && b[0] == 255 && b[1] == 216 && b[2] == 255) return ".jpg";
        if (b.Length >= 12 && b[..4].SequenceEqual("RIFF"u8) && b.Slice(8, 4).SequenceEqual("WEBP"u8)) return ".webp";
        return null;
    }
    public static bool IsAllowedUrl(string? value) => !string.IsNullOrWhiteSpace(value) && (System.Text.RegularExpressions.Regex.IsMatch(value, @"^/images/cars/[a-zA-Z0-9-]+\.(png|jpg|jpeg|webp)$") || (Uri.TryCreate(value, UriKind.Absolute, out var u) && u.Scheme == "https"));
}
