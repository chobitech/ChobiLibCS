using System.Security.Cryptography;

namespace Chobitech;

public static class ChobiLib
{
    public const double PI2 = Math.PI * 2.0;


    private static readonly Lazy<RandomNumberGenerator> lazyRng = new(() => RandomNumberGenerator.Create());
    public static RandomNumberGenerator SharedRandomNumberGenerator => lazyRng.Value;

    public static byte[] GenerateRandomBytes(int size)
    {
        var res = new byte[size];
        SharedRandomNumberGenerator.GetBytes(res);
        return res;
    }


    public static T Also<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    public static T AlsoIf<T>(this T t, bool isExecAction, Action<T> action)
    {
        if (isExecAction)
        {
            action(t);
        }
        return t;
    }

    public static R Let<T, R>(this T t, Func<T, R> func) => func(t);

    public static IEnumerable<T> Repeat<T>(this T t, int count) => Enumerable.Repeat(t, count);
}
