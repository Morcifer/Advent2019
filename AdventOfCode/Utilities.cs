namespace AdventOfCode;

public static class Utilities
{
    public static IEnumerable<int> AllIndexesOf(this string str, string substring)
    {
        for (var index = 0; ; index += substring.Length)
        {
            index = str.IndexOf(substring, index, StringComparison.CurrentCulture);

            if (index == -1)
            {
                break;
            }

            yield return index;
        }
    }

    public static IEnumerable<(int Index, T Value)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((v, i) => (i, v)); // Sure would have been nicer if I could just do this in the foreach directly...
    }

    public static IEnumerable<T> Reversed<T>(this List<T> source)
    {
        for (int i = source.Count - 1; i >= 0; i--)
        {
            yield return source[i];
        }
    }

    public static IEnumerable<(T, T)> Windows<T>(this List<T> source)
    {
        for (int i = 0; i < source.Count - 1; i++)
        {
            yield return (source[i], source[i + 1]);
        }
    }

    public static int Product(this IEnumerable<int> source)
    {
        return source.Aggregate(1, (x, y) => x * y);
    }

    public static long Product(this IEnumerable<long> source)
    {
        return source.Aggregate((long)1, (x, y) => x * y);
    }

    public static IEnumerable<List<string>> Cluster(this IEnumerable<string> text)
    {
        var cluster = new List<string>();

        foreach (var line in text)
        {
            if (line == "")
            {
                yield return cluster;
                cluster = new List<string>();
            }
            else
            {
                cluster.Add(line);
            }
        }

        yield return cluster;
    }
}
