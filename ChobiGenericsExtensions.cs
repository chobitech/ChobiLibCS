using System.Diagnostics.CodeAnalysis;

namespace Chobitech;

public static class ChobiGenericsExtensions
{
    public static bool IsDefault<T>([NotNullWhen(false)] this T t)
    {
        return EqualityComparer<T>.Default.Equals(t, default!);
    }
}
