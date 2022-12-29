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

    public static IEnumerable<(T First, T Second)> Pair<T>(this IEnumerable<T> seq)
    {
        using var enumerator = seq.GetEnumerator();
        enumerator.MoveNext();
        var first = enumerator.Current;
        do {
            if (!enumerator.MoveNext()) yield break;
            var second = enumerator.Current;
            yield return (first, second);
            first = second;

        } while (true);
    }

    public static void Each<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items) action(item);
    }

    public static void Each<T>(this IEnumerable<T> items, Action<T, int> action)
    {
        using var enumerator = items.GetEnumerator();
        for (int i = 0; enumerator.MoveNext(); i++) {
            action(enumerator.Current, i);
        }
    }

    public static IEnumerable<T> TakeAt<T>(this IEnumerable<T> items, params int[] indexes)
    {
        using var enumerator = items.GetEnumerator();
        for (int i = 0; enumerator.MoveNext(); i++) {
            if (indexes.Contains(i)) 
            yield return enumerator.Current;
        }
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

public record struct Point(int X, int Y)
{
    public static readonly Point Zero = new(0, 0);

    public Point(string X, string Y) : this(int.Parse(X), int.Parse(Y))
    {
    }

    public static implicit operator Point((int, int) p) => new(p.Item1, p.Item2);

    public static Point operator +(Point left, Point right)
        => new(X: left.X + right.X, Y: left.Y + right.Y);

    public static Point operator -(Point left, Point right)
        => new(X: left.X - right.X, Y: left.Y - right.Y);

    public static Point operator /(Point left, Point right)
        => new(X: left.X / Math.Max(right.X, 1), Y: left.Y / Math.Max(right.Y, 1));

    public Point Abs() => new(Math.Abs(X), Math.Abs(Y));
    public int Sum() => X+Y;
}
