using System.Numerics;

namespace Chobitech;

public class ValueRange<T> where T : IComparable<T>
{
    public enum RangeCompareResult
    {
        InRange,
        BelowRange,
        AboveRange,
        AtMin,
        AtMax,
    }

    public readonly T minValue;
    public readonly T maxValue;

    public ValueRange(T minValue, T maxValue)
    {
        var comp = minValue.CompareTo(maxValue);

        if (comp == 0)
        {
            throw new ArgumentException($"{nameof(minValue)} and {nameof(maxValue)} values are same, or ");
        }
        
        if (comp > 0)
        {
            throw new ArgumentException($"{nameof(minValue)} is greater than {nameof(maxValue)}");
        }

        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public RangeCompareResult CheckInRange(T value)
    {
        return value.CompareTo(minValue) switch
        {
            < 0 => RangeCompareResult.BelowRange,
            0 => RangeCompareResult.AtMin,
            _ => value.CompareTo(maxValue) switch
            {
                > 0 => RangeCompareResult.AboveRange,
                0 => RangeCompareResult.AtMax,
                _ => RangeCompareResult.InRange,
            }
        };
    }

    public bool IsInRange(T value, bool includeMinValue = true, bool includeMaxValue = true)
    {
        return CheckInRange(value) switch
        {
            RangeCompareResult.InRange => true,
            RangeCompareResult.AtMin => includeMinValue,
            RangeCompareResult.AtMax => includeMaxValue,
            _ => false,
        };
    }


    public override string ToString()
    {
        return $"min = {minValue}, max = {maxValue}";
    }
}

public static class ValueRangeExtensions
{


    public static BigInteger GetValueGap(this ValueRange<BigInteger> range) => range.maxValue - range.minValue;

    public static short GetValueGap(this ValueRange<short> range) => (short)(range.maxValue - range.minValue);
    public static byte GetValueGap(this ValueRange<byte> range) => (byte)(range.maxValue - range.minValue);

    public static int GetValueGap(this ValueRange<int> range) => range.maxValue - range.minValue;    
    public static float GetValueGap(this ValueRange<float> range) => range.maxValue - range.minValue;
    public static long GetValueGap(this ValueRange<long> range) => range.maxValue - range.minValue;
    public static double GetValueGap(this ValueRange<double> range) => range.maxValue - range.minValue;
    public static decimal GetValueGap(this ValueRange<decimal> range) => range.maxValue - range.minValue;

}
