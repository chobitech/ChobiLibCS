namespace ChobiLib;

public static class ChobiLib
{
    public const double PI2 = Math.PI * 2.0;


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
