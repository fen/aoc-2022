if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day4 <input>");
    return;
}

var pairs = await ParseAsync(file);
Console.WriteLine(
    "Part One: {0}", 
    pairs.Count(p => p.IsFullyContained)
);
Console.WriteLine(
    "Part Two: {0}", 
    pairs.Count(p => p.Overlap)
);

async Task<List<Pair>> ParseAsync(string file)
    => (await File.ReadAllLinesAsync(file)).Select(Pair.Parse).ToList();

public record struct Pair(Sections First, Sections Second)
{
    public static Pair Parse(string value)
    {
        var (first, second) = value.Split(',');
        return new(
            Sections.Parse(first),
            Sections.Parse(second)
        );
    }

    public bool IsFullyContained => First.FullyContains(Second) || Second.FullyContains(First);
    public bool Overlap => First.Overlap(Second);
}

public record struct Sections(int Start, int End)
{
    public static Sections Parse(string value)
    {
        var (start, end) = value.Split('-');
        return new(int.Parse(start), int.Parse(end));
    }

    public bool Overlap(Sections other) 
        => Start <= other.End && End >= other.Start;
    public bool FullyContains(Sections other) 
        => Start <= other.Start && End >= other.End;
}
