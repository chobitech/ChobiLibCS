using System.Text;

namespace Chobitech;

public static class ListAndArrayUtils
{
    public static int LastIndex<T>(this IEnumerable<T> list) => list.Count() - 1;

    public static bool IsNotEmpty<T>(this IEnumerable<T> list) => list.Any();

    public static string JoinToString<T>(this IEnumerable<T> values, string joint = "", Func<T, string>? converter = null)
    {
        var sb = new StringBuilder();
        foreach (var v in values)
        {
            if (joint != null && joint != "" && sb.IsNotEmpty())
            {
                sb.Append(joint);
            }
            sb.Append(v);
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

    public static R FoldIndexed<T, R>(this IEnumerable<T> list, R initialValue, Func<R, T, int, R> func)
    {
        var res = initialValue;

        for (var i = 0; i < list.Count(); i++)
        {
            res = func(res, list.ElementAt(i), i);
        }

        return res;
    }

    public static R Fold<T, R>(this IEnumerable<T> list, R initialValue, Func<R, T, R> func) => list.FoldIndexed(initialValue, (res, v, _) => func(res, v));

    public static void ForeachIndexed<T>(this IEnumerable<T> list, Action<T, int> action)
    {
        for (var i = 0; i < list.Count(); i++)
        {
            action(list.ElementAt(i), i);
        }
    }

    public static List<R> MapIndexed<T, R>(this IEnumerable<T> list, Func<T, int, R> func, bool ignoreNull = false) where T : class
    {
        var res = new List<R>();

        for (var i = 0; i < list.Count(); i++)
        {
            var v = list.ElementAt(i);
            var r = func(v, i);
            if (!ignoreNull || v != null)
            {
                res.Add(r);
            }
        }

        return res;
    }

    public static List<R> Map<T, R>(this IEnumerable<T> list, Func<T, R> func, bool ignoreNull = false) where T : class => list.MapIndexed((v, _) => func(v), ignoreNull);

    public static List<R> MapIndexed<T, R>(this IEnumerable<T> list, Func<T, int, R> func) where T : struct
    {
        var res = new List<R>();

        for (var i = 0; i < list.Count(); i++)
        {
            res.Add(func(list.ElementAt(i), i));

        }

        return res;
    }

    public static List<R> Map<T, R>(this IEnumerable<T> list, Func<T, R> func) where T : struct => list.MapIndexed((v, _) => func(v));

    public static int[] GetIndices<T>(IEnumerable<T> values)
    {
        var indices = new int[values.Count()];
        for (var i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }
        return indices;
    }

    public static IntRange? GetIndexRange<T>(IEnumerable<T> values)
    {
        var lastIndex = values.LastIndex();
        if (lastIndex < 0)
        {
            return null;
        }
        return new(0, lastIndex);
    }

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
}
