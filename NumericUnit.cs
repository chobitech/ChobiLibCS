using System.Numerics;

namespace ChobiLib;

public class NumericUnit
{
    private static readonly IComparer<int> intDescComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
    private static readonly BigInteger siUnitBase = BigInteger.Pow(10, 3);
    private static readonly BigInteger jpUnitBase = BigInteger.Pow(10, 4);


    private static readonly Dictionary<int, BigInteger> digitsAndBaseValueMap = [];
    private static BigInteger GetBaseValue(int digits)
    {
        if (digits < 3)
        {
            return BigInteger.One;
        }

        if (!digitsAndBaseValueMap.TryGetValue(digits, out BigInteger bv))
        {
            bv = BigInteger.Pow(10, digits);
            digitsAndBaseValueMap[digits] = bv;
        }
        return bv;
    }


    private readonly struct UnitData(
        string label,
        string reading)
    {
        public string Label { get; } = label;
        public string Reading { get; } = reading;
    }

    private static readonly SortedDictionary<int, UnitData> siUnitMap = new(intDescComparer)
    {
        { 24, new("Y", "yotta") },
        { 21, new("Z", "zetta") },
        { 18, new("E", "exa") },
        { 15, new("P", "peta") },
        { 12, new("T", "tera") },
        { 9, new("G", "giga") },
        { 6, new("M", "mega") },
        { 3, new("K", "kilo") },
    };

    private static readonly SortedDictionary<int, UnitData> jpUnitMap = new(intDescComparer)
    {
        { 68, new("無量大数", "muryoutaisuu") },
        { 64, new("不可思議", "fukashigi") },
        { 60, new("那由多", "nayuta") },
        { 56, new("阿僧祇", "asougi") },
        { 52, new("恒河沙", "gougasha") },
        { 48, new("極", "goku") },
        { 44, new("載", "sai") },
        { 40, new("正", "sei") },
        { 36, new("澗", "kan") },
        { 32, new("溝", "kou") },
        { 28, new("穣", "jou") },
        { 24, new("秭", "jo") },
        { 20, new("垓", "gai") },
        { 16, new("京", "kei") },
        { 12, new("兆", "chou") },
        { 8, new("億", "oku") },
        { 4, new("万", "man") },
    };

    public static int CountDigits(BigInteger bi)
    {
        return (int)Math.Ceiling(BigInteger.Log10(bi));
    }



    public static string GetExponentialString(BigInteger bi, int exponentDigits = 6, bool addComma = false)
    {
        var digits = CountDigits(bi);
        if (digits < exponentDigits)
        {
            return addComma ? bi.ToString("#,0") : bi.ToString();
        }
        else
        {
            digits--;
            var fBase = GetBaseValue(digits - 1);
            var v = bi / fBase;
            var i = v / 10;
            var f = v % 10;
            return $"{i}.{f}e{digits}";
        }
    }

    public static string GetUnitString(BigInteger bi, bool addComma = false)
    {
        //--- en
        var digits = CountDigits(bi);
        var unit = "";
        var fPart = BigInteger.Zero;

        foreach (var uData in siUnitMap)
        {
            if (digits >= uData.Key)
            {
                var baseValue = GetBaseValue(uData.Key - 3);
                var reBi = bi / baseValue;
                fPart = reBi % siUnitBase;
                bi = reBi / siUnitBase;
                unit = uData.Value.Label;
                break;
            }
        }

        return $"{bi.ToString(addComma)}.{fPart}{unit}";
    }


}
