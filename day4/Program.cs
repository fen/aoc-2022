if (args is not [var file]) {
    Exit("day4 <input>");
    return;
}

if (!File.Exists(file)) Exit($"File {file} does not exist");

var pairs = await ParseAsync(file);
Console.WriteLine("Part One: {0}", pairs.Count(p => p.IsFullyContained));
Console.WriteLine("Part Two: {0}", pairs.Count(p => p.Overlap));

async Task<List<Pair>> ParseAsync(string file)
    => (await File.ReadAllLinesAsync(file)).Select(Pair.Parse).ToList();

[DoesNotReturn]
static void Exit(string msg)
{
    Console.WriteLine(msg);
    Environment.Exit(-1);
}

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

    public bool Overlap(Sections other) => Start <= other.End && End >= other.Start;
    public bool FullyContains(Sections other) => Start <= other.Start && End >= other.End;
}

static class Ext
{
    public static void Deconstruct<T>(this T[] seq, out T first, out T second)
    {
        first = seq[0];
        second = seq[1];
    }
}
