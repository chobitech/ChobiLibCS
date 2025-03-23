namespace ChobiLib;

public class Randomize<T>(IList<T> seeds, Random? random = null)
{
    private readonly Random random = random ?? new Random();
    public T[] Seeds { get; private set; } = [..seeds];

    public T Next() => Seeds[random.Next(Seeds.Length)];

    public List<T> GetRandomList(int size, bool hasDuplicate = true)
    {
        var res = new List<T>();

        if (hasDuplicate)
        {
            for (var i = 0; i < size; i++)
            {
                res.Add(Next());
            }
        }
        else
        {
            var maxSize = Math.Min(size, Seeds.Length);
            var tempList = new List<T>(Seeds);

            for (var i = 0; i < maxSize; i++)
            {
                var index = random.Next(tempList.Count);
                res.Add(tempList[index]);
                tempList.RemoveAt(index);
            }
        }

        return res;
    }
}