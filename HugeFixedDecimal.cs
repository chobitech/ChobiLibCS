using System.Numerics;
using System.Text.RegularExpressions;
using ChobiLib;

namespace ChobiLib;

public sealed class HugeFixedDecimal
{
    private static Exception GenerateInvalidNumberFormatException(string input) => throw new($"Invalid number format: \"{input}\"");

    private static readonly Regex numberRegex = new(@"^(-?[0-9]+)(?:\.([0-9]+))?$");
    private static readonly Regex scientificNumberRegex = new(@"(-?[0-9]+(?:\.[0-9]+)?)[eE]([0-9]+)$");

    private static (int, BigInteger)? ParseFromDoubleString(string doubleString)
    {
        var match = numberRegex.Match(doubleString);
        if (!match.Success)
        {
            return null;
        }

        var intPart = BigInteger.Parse(match.Groups[1].Value);

        var fracStr = match.Groups[2].Value;
        int fracDigits = fracStr.Length;

        var fracBi = BigInteger.Parse(fracStr);
        if (intPart.Sign < 0)
        {
            fracBi *= -1;
        }

        return (fracDigits, (intPart * GetInnerValuesHolder(fracDigits).exp10) + fracBi);
    }

    private static (int, BigInteger)? ParseFromScientificString(string s)
    {
        var match = scientificNumberRegex.Match(s);
        if (!match.Success)
        {
            return null;
        }

        var fracPartResult = ParseFromDoubleString(match.Groups[1].Value);
        if (fracPartResult == null)
        {
            return null;
        }

        var exp10Digits = int.Parse(match.Groups[2].Value);

        int fDigits = 0;
        if (fracPartResult.Value.Item1 > exp10Digits)
        {
            fDigits = fracPartResult.Value.Item1 - exp10Digits;
        }

        var valueHolder = GetInnerValuesHolder(fracPartResult.Value.Item1 + exp10Digits);

        return (fDigits, fracPartResult.Value.Item2 * valueHolder.exp10);
    }

    public static HugeFixedDecimal Parse(string str, int? fractionalDigits = null)
    {
        var parseResult = ParseFromDoubleString(str);
        if (parseResult == null)
        {
            parseResult = ParseFromScientificString(str);
            if (parseResult == null)
            {
                throw GenerateInvalidNumberFormatException(str);
            }
        }

        var hfd = new HugeFixedDecimal(parseResult.Value.Item2, parseResult.Value.Item1);

        if (fractionalDigits != null && parseResult.Value.Item1 != fractionalDigits)
        {
            hfd = hfd.ChangeFractionalDigits(fractionalDigits.Value);
        }
        return hfd;
    }

    public static bool TryParse(string str, out HugeFixedDecimal? value, int? fractionalDigits = null)
    {
        try
        {
            value = Parse(str, fractionalDigits);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }


    private class InnerValuesHolder
    {
        public readonly double exp10Double;
        public readonly BigInteger exp10;

        public InnerValuesHolder(
            double exp10Double,
            BigInteger exp10)
        {
            this.exp10Double = exp10Double;
            this.exp10 = exp10;
        }
    }

    public const int DefaultFractionalDigits = 5;

    private static readonly BigInteger bi10 = new(10);

    private static readonly Dictionary<int, InnerValuesHolder> exp10Map = new()
    {
        { 1, new(10, bi10) },
    };

    private static InnerValuesHolder GetInnerValuesHolder(int exp)
    {
        lock (exp10Map)
        {
            if (!exp10Map.TryGetValue(exp, out InnerValuesHolder? holder))
            {
                holder = new(
                    Math.Pow(10, exp),
                    BigInteger.Pow(bi10, exp)
                );
                exp10Map[exp] = holder;
            }
            return holder;
        }
    }

    public int FractionalDigits { get; }
    private readonly InnerValuesHolder valuesHolder;

    private BigInteger GetDoubleToBigInt(double d) => new(d * valuesHolder.exp10Double);

    public BigInteger RawFullValue { get; private set; }

    public HugeFixedDecimal(BigInteger rawFullValue, int fractionalDigits = DefaultFractionalDigits)
    {
        FractionalDigits = fractionalDigits;
        valuesHolder = GetInnerValuesHolder(fractionalDigits);
        RawFullValue = rawFullValue;
    }

    public HugeFixedDecimal(double d, int fractionalDigits = DefaultFractionalDigits)
    {
        FractionalDigits = fractionalDigits;
        valuesHolder = GetInnerValuesHolder(fractionalDigits);
        RawFullValue = GetDoubleToBigInt(d);
    }


    private HugeFixedDecimal CalcWithDouble(double d, Func<BigInteger, BigInteger, BigInteger> calc)
    {
        var dBi = GetDoubleToBigInt(d);
        return new(calc(RawFullValue, dBi), FractionalDigits);
    }


    public HugeFixedDecimal Add(double d) => CalcWithDouble(d, (raw, dBi) => raw + dBi);
    public HugeFixedDecimal Subtract(double d) => Add(-d);
    public HugeFixedDecimal Multiply(double d) => CalcWithDouble(d, (raw, dBi) => raw * dBi / valuesHolder.exp10);
    public HugeFixedDecimal Divide(double d) => CalcWithDouble(d, (raw, dBi) => raw * valuesHolder.exp10 / dBi);


    private HugeFixedDecimal CalcWithHfd(HugeFixedDecimal v, Func<BigInteger, BigInteger, InnerValuesHolder, BigInteger> calc)
    {
        var gapValueHolder = GetInnerValuesHolder(Math.Abs(FractionalDigits - v.FractionalDigits));

        var exp = Math.Max(FractionalDigits, v.FractionalDigits);
        var valueHolder = GetInnerValuesHolder(exp);

        var selfBi = RawFullValue;
        var vBi = v.RawFullValue;

        if (FractionalDigits > v.FractionalDigits)
        {
            vBi *= gapValueHolder.exp10;
        }
        else if (FractionalDigits < v.FractionalDigits)
        {
            selfBi *= gapValueHolder.exp10;
        }

        return new(calc(selfBi, vBi, valueHolder), exp);
    }


    public HugeFixedDecimal Add(HugeFixedDecimal v) => CalcWithHfd(v, (bi1, bi2, _) => bi1 + bi2);
    public HugeFixedDecimal Subtract(HugeFixedDecimal v) => CalcWithHfd(v, (bi1, bi2, _) => bi1 - bi2);
    public HugeFixedDecimal Multiply(HugeFixedDecimal v) => CalcWithHfd(v, (bi1, bi2, vh) => bi1 * bi2 / vh.exp10);
    public HugeFixedDecimal Divide(HugeFixedDecimal v) => CalcWithHfd(v, (bi1, bi2, vh) => bi1 * vh.exp10 / bi2);


    public HugeFixedDecimal ChangeFractionalDigits(int targetFractionalDigits)
    {
        if (targetFractionalDigits < 0)
        {
            throw new Exception("The input fractionalDigits is not positive");
        }

        var fracDigitsGap = FractionalDigits - targetFractionalDigits;
        BigInteger bi = RawFullValue;

        if (fracDigitsGap > 0)
        {
            bi /= GetInnerValuesHolder(fracDigitsGap).exp10;
        }
        else if (fracDigitsGap < 0)
        {
            bi *= GetInnerValuesHolder(-fracDigitsGap).exp10;
        }

        return new(bi, targetFractionalDigits);
    }


    public string ToString(bool addComma)
    {
        var absValue = BigInteger.Abs(RawFullValue);
        var intPart = absValue / valuesHolder.exp10;
        var fracPart = absValue % valuesHolder.exp10;

        var sign = RawFullValue.Sign < 0 ? "-" : "";
        var fracStr = fracPart.ToString();
        var padCount = FractionalDigits - fracStr.Length;
        if (padCount > 0)
        {
            fracStr = fracStr.PadLeft(padCount, '0');
        }

        return $"{sign}{intPart.ToString(addComma)}.{fracStr}";
    }


    public override string ToString() => ToString(false);

    public override bool Equals(object? obj)
    {
        if (obj is HugeFixedDecimal hfd)
        {
            return (hfd.FractionalDigits == FractionalDigits) && (hfd.RawFullValue == RawFullValue);
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + FractionalDigits.GetHashCode();
            hash = hash * 31 + RawFullValue.GetHashCode();
            return hash;
        }
    }
}
