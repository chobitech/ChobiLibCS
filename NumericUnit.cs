using System.Numerics;
using System.Text;

namespace ChobiLib;



public static class ChobiBigIntegerExtensions
{
    public static BigInteger Round(this BigInteger bi, int digits) => NumericUnit.GetRoundedBigInteger(bi, digits);
    public static BigInteger Ceil(this BigInteger bi, int digits) => NumericUnit.GetCeilingBigInteger(bi, digits);
    public static BigInteger Floor(this BigInteger bi, int digits) => NumericUnit.GetFlooringBigInteger(bi, digits);
}


public class NumericUnit
{
    private static readonly IComparer<int> intDescComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
    private static readonly BigInteger siUnitBase = BigInteger.Pow(10, 3);
    private static readonly BigInteger jpUnitBase = BigInteger.Pow(10, 4);


    private static readonly Dictionary<int, BigInteger> digitsAndBaseValueMap = [];

    private static BigInteger GetBaseValue(int digits)
    {
        lock (digitsAndBaseValueMap)
        {
            if (!digitsAndBaseValueMap.TryGetValue(digits, out BigInteger bv))
            {
                bv = BigInteger.Pow(10, digits);
                digitsAndBaseValueMap[digits] = bv;
            }
            return bv;
        }
    }


    private readonly struct UnitData(
        string label,
        string reading)
    {
        public string Label { get; } = label;
        public string Reading { get; } = reading;
    }

    private static readonly Dictionary<int, string> jpNumberMap = new()
    {
        { 0, "〇" },
        { 1, "一" },
        { 2, "二" },
        { 3, "三" },
        { 4, "四" },
        { 5, "五" },
        { 6, "六" },
        { 7, "七" },
        { 8, "八" },
        { 9, "九" }
    };

    private static string InnerGetJpUnitNumber(int value, int baseValue, string suffix)
    {
        if (value < baseValue)
        {
            return "";
        }

        var p = value / baseValue;
        if (p > 1)
        {
            return jpNumberMap[p] + suffix;
        }
        else
        {
            return suffix;
        }
    }

    private static string InnerGet4DigitsJpNumberString(int n)
    {
        if (n >= 10000)
        {
            throw new Exception("n over 10000");
        }

        var s = InnerGetJpUnitNumber(n, 1000, "千");
        n %= 1000;

        s += InnerGetJpUnitNumber(n, 100, "百");
        n %= 100;

        s += InnerGetJpUnitNumber(n, 10, "十");
        n %= 10;

        s += jpNumberMap[n];

        return s;
    }

    private static List<string> InnerGetJpNumberUnitList(BigInteger bi)
    {
        var iList = new List<string>();
        do
        {
            var partInt = (int)(bi % jpUnitBase);
            iList.Add(InnerGet4DigitsJpNumberString(partInt));
            bi /= jpUnitBase;
        } while (bi > BigInteger.Zero);

        return iList;
    }

    
    private static string GetShortenJpNumberString(BigInteger bi)
    {
        var sb = new StringBuilder();

        if (bi < jpUnitBase)
        {
            sb.Append(InnerGet4DigitsJpNumberString((int)bi));
        }
        else
        {
            var unitCount = jpUnitMap.Count;
            var lastUnitData = jpUnitMap.Last().Value!.Value;
            var last10Exp = jpUnitMap.Last().Key;

            var bi10Exp = GetBase10Exponent(bi);
            var exp10 = bi10Exp - (bi10Exp % 4);
            var roundBi = bi.Round(exp10 -1);

            var unitDigits = bi10Exp % last10Exp;
            var unitStr = "";
            for (var i = unitCount - 1; i > 0; i--)
            {
                var elm = jpUnitMap.ElementAt(i);
                if (unitDigits >= elm.Key && elm.Value != null)
                {
                    unitStr = elm.Value.Value.Label;
                    break;
                }
            }

            for (var i = 0; i < bi10Exp / last10Exp; i++)
            {
                unitStr += lastUnitData.Label;
            }

            var exp10BaseValue = GetBaseValue(exp10);
            var iPart = InnerGet4DigitsJpNumberString((int)(roundBi / exp10BaseValue));
            var fPart = (int)((roundBi % exp10BaseValue) / GetBaseValue(exp10 - 1));

            sb.Append(iPart).Append("点").Append(jpNumberMap[fPart]).Append(unitStr);
        }



        return sb.ToString();
    }

    public static string GetJpNumberString(BigInteger bi, bool isShorten = false)
    {
        if (isShorten)
        {
            return GetShortenJpNumberString(bi);
        }

        var numList = InnerGetJpNumberUnitList(bi);
        
        if (numList.Count == 0)
        {
            return "";
        }
        
        var sb = new StringBuilder();

        var unitCount = jpUnitMap.Count;
        var lastNumData = jpUnitMap.Last().Value!.Value;

        do
        {
            List<string> tempNumList;

            if (numList.Count > unitCount)
            {
                tempNumList = numList.GetRange(0, unitCount - 1);
                numList.RemoveRange(0, unitCount - 1);
            }
            else
            {
                tempNumList = numList.GetRange(0, numList.Count);
                numList.Clear();
            }

            for (var i = 0; i < tempNumList.Count; i++)
            {
                var unitData = jpUnitMap.ElementAt(i).Value;
                if (unitData != null)
                {
                    sb.Insert(0, unitData.Value.Label);
                }
                sb.Insert(0, tempNumList[i]);
            }

            if (numList.IsNotEmpty())
            {
                sb.Insert(0, lastNumData.Label);
            }
        } while (numList.IsNotEmpty());

        return sb.ToString();
    }

    private static readonly SortedDictionary<int, UnitData> siUnitMap = new(intDescComparer)
    {
        { 30, new("Q", "quetta") },
        { 27, new("R", "ronna") },
        { 24, new("Y", "yotta") },
        { 21, new("Z", "zetta") },
        { 18, new("E", "exa") },
        { 15, new("P", "peta") },
        { 12, new("T", "tera") },
        { 9, new("G", "giga") },
        { 6, new("M", "mega") },
        { 3, new("K", "kilo") },
    };


    private static readonly SortedDictionary<int, UnitData?> jpUnitMap = new()
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
        { 0, null },
    };

    private static UnitData? GetJpUnitData(int exp10)
    {
        for (var i = jpUnitMap.Count - 1; i > 0; i--)
        {
            var elm = jpUnitMap.ElementAt(i);
            if (elm.Value != null && exp10 >= elm.Key)
            {
                return elm.Value!.Value;
            }
        }
        return null;
    }


    private static BigInteger InnerBiValueProcess(BigInteger bi, int targetDigits, Func<BigInteger, BigInteger, bool> func)
    {
        if (targetDigits > 1)
        {
            var baseValue = GetBaseValue(targetDigits);
            var halfBaseValue = baseValue / 2;
            var mPart = bi % baseValue;
            bi -= mPart;
            if (func(mPart, halfBaseValue))
            {
                bi += baseValue;
            }
        }

        return bi;
    }


    public static BigInteger GetCeilingBigInteger(BigInteger bi, int ceilDigit) => InnerBiValueProcess(bi, ceilDigit, (m, half) => m > BigInteger.Zero);
    public static BigInteger GetRoundedBigInteger(BigInteger bi, int roundDigit) => InnerBiValueProcess(bi, roundDigit, (m, half) => m >= half);
    public static BigInteger GetFlooringBigInteger(BigInteger bi, int floorDigit) => InnerBiValueProcess(bi, floorDigit, (m, half) => false);

    
    public static int CountDigits(BigInteger bi)
    {
        return (int)Math.Ceiling(BigInteger.Log10(bi));
    }

    public static int GetBase10Exponent(BigInteger bi)
    {
        var dCount = CountDigits(bi);
        return (dCount > 0) ? dCount - 1 : 0;
    }



    public static string GetExponentialString(BigInteger bi, int exponentDigits = 6, bool addComma = false)
    {
        var digits = GetBase10Exponent(bi);
        if (exponentDigits < 1 || digits < exponentDigits)
        {
            return addComma ? bi.ToString("#,0") : bi.ToString();
        }
        else
        {
            var d = digits - 1;
            bi = GetRoundedBigInteger(bi, d);
            var fBase = GetBaseValue(d);
            var v = bi / fBase;
            var i = v / 10;
            var f = v % 10;
            return $"{i}.{f}e{digits}";
        }
    }

    public static string GetSiUnitString(BigInteger bi, bool addComma = false)
    {
        //--- en
        var digits = GetBase10Exponent(bi);
        var unit = "";
        var fPart = BigInteger.Zero;

        foreach (var uData in siUnitMap)
        {
            if (digits >= uData.Key)
            {
                var d = uData.Key - 1;
                var baseValue = GetBaseValue(uData.Key);
                bi = GetRoundedBigInteger(bi, d);
                fPart = bi % baseValue / GetBaseValue(d);
                bi /= baseValue;
                unit = uData.Value.Label;
                break;
            }
        }

        return $"{bi.ToString(addComma)}.{fPart}{unit}";
    }


}
