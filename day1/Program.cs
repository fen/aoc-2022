using System.Diagnostics.CodeAnalysis;

if (args is not [var file]) {
    Exit("day1 <input>");
    return;
}

if (!File.Exists(file)) Exit($"File {file} does not exist");

var elves = await ReadElfDataAsync(file);

Console.WriteLine(
    "Total calories: {0}",
    elves.OrderByDescending(e => e.TotalCalories).First().TotalCalories
);

Console.WriteLine(
    "Sum of top three elfs: {0}",
    elves.OrderByDescending(e => e.TotalCalories).Take(3).Sum(e => e.TotalCalories)
);

async Task<List<Elf>> ReadElfDataAsync(string file)
{
    var lines = await File.ReadAllLinesAsync(file);

    List<Elf> elfs = new();

    Elf currentElf = new();
    elfs.Add(currentElf);
    foreach (var line in lines) {
        if (line.Length == 0) {
            currentElf = new();
            elfs.Add(currentElf);
            continue;
        }

        var calories = int.Parse(line);
        currentElf.TotalCalories += calories;
        currentElf.Items.Add(calories);
    }

    return elfs;
}

[DoesNotReturn]
static void Exit(string msg)
{
    Console.WriteLine(msg);
    Environment.Exit(-1);
}

public class Elf
{
    public List<int> Items { get; } = new();
    public int TotalCalories { get; set; }
}

