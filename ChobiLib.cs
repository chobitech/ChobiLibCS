using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace ChobiLib;

public static class ChobiLib
{
    public const double PI2 = Math.PI * 2.0;


    public static T Also<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    public static R Let<T, R>(this T t, Func<T, R> func) => func(t);


    public static List<string> ToStringList(this string s)
    {
        return [.. s.ToCharArray().Select(c => c.ToString())];
    }

    public static int LastIndex<T>(this IEnumerable<T> list) => list.Count() - 1;

    public static bool IsNotEmpty<T>(this IEnumerable<T> list) => list.Any();


    public static bool IsNotEmpty(this StringBuilder sb) => sb.Length > 0;

    public static string JoinToString<T>(this IList<T> list, string joint = "", Func<T, string>? converter = null)
    {
        var sb = new StringBuilder();
        foreach (var t in list)
        {
            if (joint != "" && sb.IsNotEmpty())
            {
                sb.Append(joint);
            }
            sb.Append(t);
        }
        return sb.ToString();
    }

    public static T Replace<T>(this IList<T> list, int index, T value)
    {
        var oldValue = list[index];
        list[index] = value;
        return oldValue;
    }

    public static void Exchange<T>(this IList<T> list, int index1, int index2)
    {
        (list[index2], list[index1]) = (list[index1], list[index2]);
    }

    public static R FoldIndexed<T, R>(this IList<T> list, R initialValue, Func<R, T, int, R> func)
    {
        var res = initialValue;

        for (var i = 0; i < list.Count; i++)
        {
            res = func(res, list[i], i);
        }

        return res;
    }

    public static R Fold<T, R>(this IList<T> list, R initialValue, Func<R, T, R> func) => list.FoldIndexed(initialValue, (res, v, _) => func(res, v));

    public static void ForeachIndexed<T>(this IList<T> list, Action<T, int> action)
    {
        for (var i = 0; i < list.Count; i++)
        {
            action(list[i], i);
        }
    }

    public static List<R> MapIndexed<T, R>(this IList<T> list, Func<T, int, R> func, bool ignoreNull = false) where T : class
    {
        var res = new List<R>();

        for (var i = 0; i < list.Count; i++)
        {
            var v = list[i];
            var r = func(v, i);
            if (!ignoreNull || v != null)
            {
                res.Add(r);
            }
        }

        return res;
    }

    public static List<R> Map<T, R>(this IList<T> list, Func<T, R> func, bool ignoreNull = false) where T : class => list.MapIndexed((v, _) => func(v), ignoreNull);

    public static List<R> MapIndexed<T, R>(this IList<T> list, Func<T, int, R> func) where T : struct
    {
        var res = new List<R>();

        for (var i = 0; i < list.Count; i++)
        {
            res.Add(func(list[i], i));

        }

        return res;
    }

    public static List<R> Map<T, R>(this IList<T> list, Func<T, R> func) where T : struct => list.MapIndexed((v, _) => func(v));


    public static T[] MergeArrays<T>(params T[][] arrays)
    {
        var totalSize = arrays.Fold(0, (total, v) => total + v.Length);

        var resArr = new T[totalSize];

        var offset = 0;
        foreach (var a in arrays)
        {
            Array.Copy(a, 0, resArr, offset, a.Length);
            offset += a.Length;
        }

        return resArr;
    }

    public static T[] MergeWith<T>(this T[] array, params T[][] arrays)
    {
        var pArr = MergeArrays(arrays);
        var len = pArr.Length + array.Length;

        var res = new T[len];

        Array.Copy(array, 0, res, 0, array.Length);
        Array.Copy(pArr, 0, res, array.Length, pArr.Length);

        return res;
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

    public static byte[] ConvretFromBase64(this string base64) => Convert.FromBase64String(base64);
    public static string ConvertToBase64String(this byte[] data) => Convert.ToBase64String(data);


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
        var match = _hexRegex.Match(hex);

        if (!match.Success)
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

    /*
    private static T InnerCircularAdd<T>(this T v, T addValue, T maxInclusive, T startValue) where T : System.Numerics.INumber<T>
    {
        var res = v + addValue;
        if (res > maxInclusive)
        {
            res = startValue;
        }
        return res;
    }
    */


    public static long CircularAdd(this long v, long addValue, long maxInclusive, long startValue = 0)
    {
        var res = v + addValue;
        if (res > maxInclusive)
        {
            res = startValue;
        }
        return res;
    }

    public static int CircularAdd(this int v, int addValue, int maxInclusive, int startValue = 0)
    {
        var res = v + addValue;
        if (res > maxInclusive)
        {
            res = startValue;
        }
        return res;
    }


    public static string ToString(this BigInteger bi, bool addComma)
    {
        return addComma ? bi.ToString("#,0") : bi.ToString();
    }
}
