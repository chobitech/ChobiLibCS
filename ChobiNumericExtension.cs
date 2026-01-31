using System.Numerics;

namespace Chobitech;

public static class ChobiNumericExtension
{
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