
/*
using System.Numerics;
using System.Text.RegularExpressions;

namespace ChobiLib;

public readonly struct FixedDecimal
{
    private static readonly BigInteger bi10 = new(10);
    private static readonly BigInteger biMinusOne = new(-1);

    private static readonly Dictionary<int, BigInteger> _scaleMap = [];


    private static BigInteger GetScale(int scaleDigits)
    {
        if (_scaleMap.TryGetValue(scaleDigits, out BigInteger scaleBi))
            {
                return scaleBi;
            }
            else
            {
                var bi = BigInteger.Pow(bi10, scaleDigits);
                _scaleMap[scaleDigits] = bi;
                return bi;
            }
    }

    public static readonly Regex _doubleRegex = new(@"^(\-)?(?:(\d*)\.)?(\d+)$");

    private static BigInteger ParseToBigInteger(string s, int scaleDigits)
    {
        var match = _doubleRegex.Match(s);
        if (!match.Success)
        {
            throw new FormatException();
        }

        var intScale = GetScale(scaleDigits);

        var sig = match.Groups[1].Success ? biMinusOne : BigInteger.One;
        var v1 = match.Groups[2];
        var v2 = match.Groups[3];

        BigInteger res;

        if (v1.Success)
        {
            var intPart = (v1.Length > 0) ? BigInteger.Parse(v1.Value) * intScale : BigInteger.Zero;
            var fracStr = $"1{v2.Value.Truncate(scaleDigits).PadRight(scaleDigits, '0')}";
            var fracPart = BigInteger.Parse(fracStr);
            res = intPart + fracPart - intScale;
        }
        else
        {
            res = BigInteger.Parse(v2.Value) * intScale;
        }

        return res * sig;
    }

    public static FixedDecimal ParseString(string s, int scaleDigits)
    {
        return new(ParseToBigInteger(s, scaleDigits), scaleDigits);
    }

    public int ScaleDigits { get; }

    public readonly BigInteger Scale => GetScale(ScaleDigits);

    public BigInteger RawValue { get; }
    public byte[] RawBytes => RawValue.ToByteArray();

    public FixedDecimal(BigInteger rawValue, int scaleDigits)
    {
        RawValue = rawValue;
        ScaleDigits = scaleDigits;
    }

    public FixedDecimal(byte[] rawBytes, int scaleDigits) : this(new BigInteger(rawBytes), scaleDigits)
    {

    }
    
    public FixedDecimal(string valueString = "0.0", int scaleDigits = 5)
    {
        ScaleDigits = scaleDigits;
        RawValue = ParseToBigInteger(valueString, scaleDigits);
    }

    public FixedDecimal(double value = 0.0, int scaleDigits = 5) : this(value.ToString(), scaleDigits)
    {

    }

    public double ToDouble() => double.Parse(ToString());
    
    public override string ToString()
    {
        var rVal = (RawValue.Sign < 0) ? -RawValue : RawValue;
        var intPart = RawValue / Scale;
        var fracPart = rVal % Scale;

        var fracStr = ((fracPart == BigInteger.Zero) ? "" : $".{fracPart.ToString().PadLeft(ScaleDigits, '0')}").TrimEnd('0');

        return $"{intPart}{fracStr}";
    }


    public static FixedDecimal operator -(FixedDecimal fd) => new(-fd.RawValue, fd.ScaleDigits);

    public static FixedDecimal operator +(FixedDecimal fd1, FixedDecimal fd2) => new(fd1.RawValue + fd2.RawValue, Math.Min(fd1.ScaleDigits, fd2.ScaleDigits));
    public static FixedDecimal operator -(FixedDecimal fd1, FixedDecimal fd2) => new(fd1.RawValue + (-fd2.RawValue), Math.Min(fd1.ScaleDigits, fd2.ScaleDigits));
    public static FixedDecimal operator *(FixedDecimal fd1, FixedDecimal fd2) => new(fd1.RawValue * fd2.RawValue, Math.Min(fd1.ScaleDigits, fd2.ScaleDigits));
    public static FixedDecimal operator /(FixedDecimal fd1, FixedDecimal fd2) => new(fd1.RawValue / fd2.RawValue, Math.Min(fd1.ScaleDigits, fd2.ScaleDigits));
    public static FixedDecimal operator %(FixedDecimal fd1, FixedDecimal fd2) => new(fd1.RawValue % fd2.RawValue, Math.Min(fd1.ScaleDigits, fd2.ScaleDigits));

    public static FixedDecimal operator +(FixedDecimal fd, double d) => fd + new FixedDecimal(d, fd.ScaleDigits);
    public static FixedDecimal operator -(FixedDecimal fd, double d) => fd - new FixedDecimal(d, fd.ScaleDigits);
    public static FixedDecimal operator *(FixedDecimal fd, double d) => fd * new FixedDecimal(d, fd.ScaleDigits);
    public static FixedDecimal operator /(FixedDecimal fd, double d) => fd / new FixedDecimal(d, fd.ScaleDigits);
    public static FixedDecimal operator %(FixedDecimal fd, double d) => fd % new FixedDecimal(d, fd.ScaleDigits);
}
*/
