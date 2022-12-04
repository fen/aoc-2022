if (args is not [var file]) {
    Exit("day3 <input>");
    return;
}

if (!File.Exists(file)) Exit($"File {file} does not exist");

var rucksacks = await ParseRucksacksAsync(file);
Console.WriteLine(
    "Part One: {0}",
    rucksacks.Select(r => r.Same).Sum(ToPriority)
);

Console.WriteLine(
    "Part Two: {0}",
    rucksacks.ByThree().Sum(CalculateGroupPriority)
);

int CalculateGroupPriority((Rucksack, Rucksack, Rucksack) group)
{
    var (first, second, third) = group;
    var sameInGroup = first.AllItems
        .Intersect(second.AllItems.Intersect(third.AllItems))
        .First();
    return ToPriority(sameInGroup);
}

static int ToPriority(char c) => c switch {
    >= 'a' and <= 'z' => c - 96,
    >= 'A' and <= 'Z' => c - 38,
    _ => throw new Exception("")
};

async Task<List<Rucksack>> ParseRucksacksAsync(string file)
{
    var lines = await File.ReadAllLinesAsync(file);
    List<Rucksack> rucksacks = new();
    foreach (var line in lines) {
        var mid = line.Length/2;

        var compartmentOne = line[0..mid];
        var compartmentTwo = line[mid..];
        var same = compartmentOne.Intersect(compartmentTwo).First();
        rucksacks.Add(new(line, same));
    }
    return rucksacks;
}

[DoesNotReturn]
static void Exit(string msg)
{
    Console.WriteLine(msg);
    Environment.Exit(-1);
}

record Rucksack(string AllItems, char Same);

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
}
