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
        var i = 0;

        foreach (var e in enumerable)
        {
            yield return (i++, e);
        }
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        where TKey : notnull
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
