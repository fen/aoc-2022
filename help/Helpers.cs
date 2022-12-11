using System.Diagnostics.CodeAnalysis;
using System.Numerics;

internal static class AdventOfCodeHelpers
{
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
        if (seq.Length == 1) {
            second = default;
        } else {
            second = seq[1];
        }
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
    public static UInt128 Product(this IEnumerable<UInt128> source)
        => Product<UInt128, UInt128>(source);

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