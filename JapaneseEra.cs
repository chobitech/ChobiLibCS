namespace ChobiLib;

public enum JapaneseEraName
{
    None        = 0,
    Meiji       = 1,
    Taisho      = 2,
    Showa       = 3,
    Heisei      = 4,
    Reiwa       = 5,
}


public readonly struct JapaneseEraData : IComparable<JapaneseEraData>, IEquatable<JapaneseEraData>
{
    internal JapaneseEraData(JapaneseEraName eraName, int christianEra, int month, int day)
    {
        EraName = eraName;
        StartDateTime = new(christianEra, month, day);
    }

    public readonly JapaneseEraName EraName { get; }

    public readonly DateTime StartDateTime { get; }

    public int ChristianEra => StartDateTime.Year;
    public int Month => StartDateTime.Month;
    public int Day => StartDateTime.Day;

    public int CompareTo(JapaneseEraData other)
    {
        return StartDateTime.CompareTo(other.StartDateTime);
    }

    public override bool Equals(object? obj)
    {
        return obj is JapaneseEraData data && Equals(data);
    }

    public bool Equals(JapaneseEraData other)
    {
        return EraName == other.EraName &&
               StartDateTime == other.StartDateTime;
    }

    public override int GetHashCode()
    {
        int hashCode = 1606841848;
        hashCode = hashCode * -1521134295 + EraName.GetHashCode();
        hashCode = hashCode * -1521134295 + StartDateTime.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(JapaneseEraData left, JapaneseEraData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JapaneseEraData left, JapaneseEraData right)
    {
        return !(left == right);
    }
}


public static class JapaneseEra
{
    public static string? GetJpName(this JapaneseEraName eraName) => eraName switch
    {
        JapaneseEraName.Meiji => "明治",
        JapaneseEraName.Taisho => "大正",
        JapaneseEraName.Showa => "昭和",
        JapaneseEraName.Heisei => "平成",
        JapaneseEraName.Reiwa => "令和",
        _ => null,
    };


    private static readonly List<JapaneseEraData> eraDataList;

    static JapaneseEra()
    {
        eraDataList = [
            new(JapaneseEraName.Meiji, 1868, 1, 25),
            new(JapaneseEraName.Taisho, 1912, 7, 30),
            new(JapaneseEraName.Meiji, 1926, 12, 25),
            new(JapaneseEraName.Meiji, 1989, 1, 8),
            new(JapaneseEraName.Meiji, 2019, 5, 1),
        ];
        eraDataList.Sort((x, y) => y.CompareTo(x));
    }

    public static JapaneseEraData? GetEraData(JapaneseEraName eraName)
    {
        return eraDataList.FirstOrDefault((data) => data.EraName == eraName);
    }

    public static JapaneseEraData? GetEraData(DateTime? dateTime = null)
    {
        var dt = dateTime ?? DateTime.Now;
        return eraDataList.FirstOrDefault((data) => dt >= data.StartDateTime);
    }

    public static JapaneseEraData? GetEraData(int year, int month, int day) => GetEraData(dateTime: new DateTime(year, month, day));
}
