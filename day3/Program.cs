if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day3 <input>");
    return;
}

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

record Rucksack(string AllItems, char Same);
