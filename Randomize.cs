namespace ChobiLib;

public class Radomize<T>(IList<T> seeds, Random? random = null)
{
    private Random random = random ?? new Random();
    public T[] Seeds { get; private set; } = [..seeds];

    public T Next() => Seeds[random.Next(Seeds.Length)];
}