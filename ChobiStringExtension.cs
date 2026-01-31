using System.Text;

namespace Chobitech;

public static class ChobiStringExtension
{
    public static bool IsNotEmpty(this StringBuilder sb) => sb.Length > 0;

    public static List<string> ToStringList(this string s)
    {
        return [.. s.ToCharArray().Select(c => c.ToString())];
    }

    public static string Truncate(this string s, int length)
    {
        if (s.Length <= length)
        {
            return s;
        }

        else
        {
#if NETSTANDARD2_0
                return s.Substring(0, length);
#else
            return s[..length];
#endif
        }
    }

    public static bool IsEmpty(this string s) => s.Length == 0;
    public static bool IsBlank(this string s) => s.TrimEnd(' ', '\r', '\n', '\t').Length == 0;


    public static byte[] ConvertToByteArray(this string s, Encoding? encoding = null) => (encoding ?? Encoding.UTF8).GetBytes(s);
    public static string ConvertToString(this byte[] data, Encoding? encoding = null) => (encoding ?? Encoding.UTF8).GetString(data);

    public static byte[] ConvertFromBase64(this string base64) => Convert.FromBase64String(base64);
    public static string ConvertToBase64String(this byte[] data) => Convert.ToBase64String(data);
}