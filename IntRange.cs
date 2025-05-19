namespace ChobiLib;

#if NETSTANDARD2_0_OR_GREATER

public readonly struct IntRange
{
    public int MinValue { get; }
    public int MaxValue { get; }

    public IntRange(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
        {
            throw new Exception($"Invalid value range: minValue({minValue}) >= maxValue({maxValue})");
        }

        MinValue = minValue;
        MaxValue = maxValue;
    }

    public RangeCompareResult CheckInRange(int value, bool includeMinValue = true, bool includeMaxValue = true)
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
        if (obj is IntRange range)
        {
            return (MinValue == range.MinValue) && (MaxValue == range.MaxValue);
        }
        return false;
    }

    public override int GetHashCode()
    {
        var hash = 17;
        hash = hash * 23 + MinValue.GetHashCode();
        hash = hash * 23 + MaxValue.GetHashCode();
        return hash;
    }
}


#elif NET8_0_OR_GREATER

public class IntRange : NumericRange<int>
{
    public IntRange(int minValue, int maxValue) : base(minValue, maxValue)
    {
    }
}

#endif
