using System.Text;

if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day25 <input>");
    return;
}

Console.WriteLine("Part One: {0}", SolveOne(file));

static List<long> ReadInput(string file)
{
    return File.ReadAllLines(file)
        .Select(line => line.Aggregate(0L, (amount, c) => amount * 5 + c switch { '2' => 2, '1' => 1, '-' => -1, '=' => -2, _ => 0 }))
        .ToList();
}

static string SolveOne(List<long> numbers)
{
    long total = numbers.Sum();
    const string characters = "012=-";
    StringBuilder result = new();
    while (total > 0) {
        result.Insert(0, characters[(int)(total % 5)]);
        total = (long)Math.Round(total / 5d, MidpointRounding.AwayFromZero);
    }
    return $"{result}";
}
