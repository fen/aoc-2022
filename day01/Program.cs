if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day1 <input>");
    return;
}

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
            elfs.Add(currentElf = new());
            continue;
        }

        var calories = int.Parse(line);
        currentElf.TotalCalories += calories;
        currentElf.Items.Add(calories);
    }

    return elfs;
}

public class Elf
{
    public List<int> Items { get; } = new();
    public int TotalCalories { get; set; }
}

