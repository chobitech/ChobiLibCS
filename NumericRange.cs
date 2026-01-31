#if NET7_0_OR_GREATER

using System.Numerics;

namespace Chobitech;

public class NumericRange<T> where T : INumber<T>
{
    public NumericRange(T minValue, T maxValue)
    {
        if (minValue >= maxValue)
        {
            throw new Exception($"Invalid value range: minValue({minValue}) >= maxValue({maxValue})");
        }

        MinValue = minValue;
        MaxValue = maxValue;
    }

    public T MinValue { get; }
    public T MaxValue { get; }

    public RangeCompareResult CheckRange(T value, bool includeMinValue = true, bool includeMaxValue = true)
    {
        if (value < MinValue || (value == MinValue && !includeMinValue))
        {
            return RangeCompareResult.BelowMin;
        }
        else if (value > MaxValue || (value == MaxValue && !includeMaxValue))
        {
            return RangeCompareResult.AboveMax;
        }
        else
        {
            return RangeCompareResult.Contains;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is NumericRange<T> nr)
        {
            return (nr.MinValue == MinValue) && (nr.MaxValue == MaxValue);
        }
        return false;
    }

    public override string ToString()
    {
        return $"[{MinValue}, {MaxValue}]";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinValue, MaxValue);
    }
}

#endif
