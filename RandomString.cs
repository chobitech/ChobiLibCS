namespace ChobiLib;

public class RandomString(IList<string> seeds, Random? random = null) : Randomize<string>([.. seeds], random)
{
    private const string digits = "0123456789";
    private const string lowerHexString = "abcdef";
    private const string lowerNonHexString = "ghijklmnopqrstuvwxyz";


    public static RandomString GetDigitsOnlyRandomString(Random? random = null) => new(digits, random);

    public static RandomString GetLowerHexRandomString(Random? random = null) => new(digits + lowerHexString, random);
    public static RandomString GetUpperHexRandomString(Random? random = null) => new(digits + lowerHexString.ToUpper(), random);
    public static RandomString GetHexRandomString(Random? random = null) => new(digits + lowerHexString + lowerHexString.ToUpper(), random);

    public static RandomString GetLowerCharDigitsRandomString(Random? random = null) => new(digits + lowerHexString + lowerNonHexString, random);
    public static RandomString GetUpperCharDigitsRandomString(Random? random = null) => new(digits + (lowerHexString + lowerNonHexString).ToUpper(), random);
    public static RandomString GetCharDigitsRandomString(Random? random = null) => (lowerHexString + lowerNonHexString).Let(cStr => new RandomString(digits + cStr + cStr.ToUpper(), random));


    public RandomString(string s, Random? random = null) : this(s.ToStringList(), random)
    {

    }

    public string GetRandomString(int size, bool hasDuplicate = true) => GetRandomList(size, hasDuplicate).JoinToString();
}