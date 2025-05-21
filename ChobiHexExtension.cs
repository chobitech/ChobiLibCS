using System.Text;
using System.Text.RegularExpressions;

namespace ChobiLib;

public static class ChobiHexExtension
{
    public static string ToHexString(this byte[] data)
    {
        var sb = new StringBuilder();

        foreach (var b in data)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }

    private static readonly Regex _hexRegex = new(@"^(?:[0-9a-f]{2})+$", RegexOptions.IgnoreCase);

    public static byte[] HexToBytes(this string hex)
    {
        if (!_hexRegex.IsMatch(hex))
        {
            throw new FormatException("The input is not hex string");
        }

        var res = new List<byte>();

        for (var i = 0; i < hex.Length; i += 2)
        {
            var b = byte.Parse($"{hex[i]}{hex[i + 1]}", System.Globalization.NumberStyles.HexNumber);
            res.Add(b);
        }

        return [.. res];
    }
}