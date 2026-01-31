
namespace Chobitech;

public static class ChobiEnumHelper
{
    public static List<T> GetEnumValues<T>(Func<T, bool>? filter = null) where T : Enum
    {
        var arr = (T[])Enum.GetValues(typeof(T));
        if (filter != null)
        {
            return [.. arr.Where(filter)];
        }
        else
        {
            return [.. arr];
        }
    }
}
