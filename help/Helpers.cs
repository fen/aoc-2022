using System.Diagnostics.CodeAnalysis;
using System.Numerics;

internal static class AdventOfCodeHelpers
{
    /// <summary>
    /// Group IEnumerable sequence in a enumerable tuple of three items.
    /// </summary>
    public static IEnumerable<(T First, T Second)> ByTwo<T>(this IEnumerable<T> seq)
    {
        using var enumerator = seq.GetEnumerator();
        do {
            if (!enumerator.MoveNext()) yield break;
            var first = enumerator.Current;
            enumerator.MoveNext();
            var second = enumerator.Current;
            yield return (first, second);
        } while (true);
    }

    /// <summary>
    /// Group IEnumerable sequence in a enumerable tuple of three items.
    /// </summary>
    public static IEnumerable<(T first, T second, T third)> ByThree<T>(this IEnumerable<T> seq)
    {
        using var enumerator = seq.GetEnumerator();
        do {
            if (!enumerator.MoveNext()) yield break;
            var first = enumerator.Current;
            enumerator.MoveNext();
            var second = enumerator.Current;
            enumerator.MoveNext();
            var third = enumerator.Current;
            yield return (first, second, third);
        } while (true);
    }

    public static void Deconstruct<T>(this T[] seq, out T first, out T second)
    {
        first = seq[0];
        second = seq.Length == 1 ? default! : seq[1];
    }

    public static bool TryRemoveFirst<T>(this List<T> items, [NotNullWhen(true)] out T? item)
    {
        if (items.Count == 0) {
            item = default;
            return false;
        }

        item = items[0];
        if (item is null) throw new();
        items.RemoveAt(0);
        return true;
    }

    public static long Product(this IEnumerable<long> source)
        => Product<long, long>(source);
    public static int Product(this IEnumerable<int> source)
        => Product<int, int>(source);

    private static TResult Product<TSource, TResult>(this IEnumerable<TSource> source)
        where TSource : struct, INumber<TSource>
        where TResult : struct, INumber<TResult>
    {
        TResult sum = TResult.One;
        foreach (TSource value in source) {
            checked { sum *= TResult.CreateChecked(value); }
        }

        return sum;
    }
}
