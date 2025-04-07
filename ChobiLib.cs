using System.Text;

namespace ChobiLib;

public static class ChobiLib
{

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


    public static bool IsNotEmpty(this StringBuilder sb) => sb.Length > 0;

    public static string JoinToString<T>(this IList<T> list, string joint = "", Func<T, string>? converter = null)
    {
        var sb = new StringBuilder();

        foreach (var t in list)
        {
            if (joint.Length > 0 && sb.IsNotEmpty())
            {
                sb.Append(joint);
            }
            
            sb.Append(converter?.Invoke(t) ?? t?.ToString());
        }

        return sb.ToString();;
    }

}
