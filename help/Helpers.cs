internal static class Ext
{
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
        } while(true);
    }

    public static void Deconstruct<T>(this T[] seq, out T first, out T second)
    {
        first = seq[0];
        second = seq[1];
    }
}
