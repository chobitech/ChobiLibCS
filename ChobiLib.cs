namespace ChobiLib;

public static class ChobiLib
{

    public static T Also<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    public static R Let<T, R>(this T t, Func<T, R> func) => func(t);

}
