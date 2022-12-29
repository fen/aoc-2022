if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day19 <input>");
    return;
}

var input = await ReadAsync(file);

Console.WriteLine(SolveOne(input));
Console.WriteLine(SolveTwo(input));

int SolveOne(IEnumerable<Blueprint> input)
{
    var sum = 0;

    input.Each((x, i) => {
        var geodes = FindGeode(x, 24);
        sum += geodes * (i + 1);

        Console.WriteLine($"Blueprint {i + 1} - {geodes} geodes");
    });

    return sum;
}

int SolveTwo(IEnumerable<Blueprint> input)
{
    var result = input.Take(3).Select(i => FindGeode(i, 32)).ToArray();
    foreach (var i in result) {
        Console.WriteLine($"{i} geodes");
    }
    return result.Product();
}

static async Task<IEnumerable<Blueprint>> ReadAsync(string file)
{
    return (await File.ReadAllLinesAsync(file)).Select(line => {
        var d = line.Split(' ')
            .TakeAt(6, 12, 18, 21, 27, 30)
            .Select(int.Parse)
            .ToArray();

        return new Blueprint(
            new(d[0], 0, 0),
            new(d[1], 0, 0),
            new(d[2], d[3], 0),
            new(d[4], 0, d[5])
        );
    });
}

static int FindGeode(Blueprint blueprint, int t)
{
    var bestGeodeCount = 0;

    Cost max = new(
        Math.Max(Math.Max(Math.Max(blueprint.Ore.Ore, blueprint.Clay.Ore), blueprint.Obsidian.Ore), blueprint.Geode.Ore),
        blueprint.Obsidian.Clay,
        blueprint.Geode.Obsidian
    );

    var queue = new Queue<(int elapsed, Robots robots, Resources resources, Skip skip)>();
    queue.Enqueue((0, default(Robots) with { Ore = 1 }, default, default));

    while (queue.TryDequeue(out var current)) {
        var (elapsed, robots, resources, skip) = current;

        var nextResources = resources with {
            Ore = resources.Ore + robots.Ore,
            Clay = resources.Clay + robots.Clay,
            Obsidian = resources.Obsidian + robots.Obsidian,
            Geode = resources.Geode + robots.Geode
        };

        if (elapsed == (t - 1)) {
            bestGeodeCount = Math.Max(nextResources.Geode, bestGeodeCount);
            continue;
        }

        var remaining = t - elapsed;

        var canBuildOre = resources.Ore >= blueprint.Ore.Ore
                        && resources.Ore + (remaining * robots.Ore) <= remaining * max.Ore;

        var canBuildClay = resources.Ore >= blueprint.Clay.Ore
                        && resources.Clay + (remaining * robots.Clay) <= remaining * max.Clay;

        var canBuildObsidian = resources.Ore >= blueprint.Obsidian.Ore
                        && resources.Clay >= blueprint.Obsidian.Clay
                        && resources.Obsidian + (remaining * robots.Obsidian) <= remaining * max.Obsidian;

        var canBuildGeode = resources.Ore >= blueprint.Geode.Ore
                        && resources.Obsidian >= blueprint.Geode.Obsidian;

        if (resources.Ore <= max.Ore) {
            queue.Enqueue((
                elapsed + 1, 
                robots, 
                nextResources, 
                //new(canBuildOre, canBuildClay, canBuildObsidian)
                default
            ));
        }

        if (canBuildOre && !skip.Ore) {
            queue.Enqueue((
                elapsed + 1,
                robots with { Ore = robots.Ore + 1 },
                nextResources with { Ore = nextResources.Ore - blueprint.Ore.Ore },
                default
            ));
        }

        if (canBuildClay && !skip.Clay) {
            queue.Enqueue((
                elapsed + 1,
                robots with { Clay = robots.Clay + 1 },
                nextResources with { Ore = nextResources.Ore - blueprint.Clay.Ore },
                default
            ));
        }

        if (canBuildObsidian && !skip.Obsidian) {
            queue.Enqueue((
                elapsed + 1,
                robots with { Obsidian = robots.Obsidian + 1 },
                nextResources with { Ore = nextResources.Ore - blueprint.Obsidian.Ore, Clay = nextResources.Clay - blueprint.Obsidian.Clay },
                default
            ));
        }

        if (canBuildGeode) {
            queue.Enqueue((
                elapsed + 1,
                robots with { Geode = robots.Geode + 1 },
                nextResources with { Ore = nextResources.Ore - blueprint.Geode.Ore, Obsidian = nextResources.Obsidian - blueprint.Geode.Obsidian },
                default
            ));
        }
    }

    return bestGeodeCount;
}

record struct Skip(bool Ore, bool Clay, bool Obsidian);
record struct Blueprint(Cost Ore, Cost Clay, Cost Obsidian, Cost Geode);
record struct Cost(int Ore, int Clay, int Obsidian);
record struct Resources(int Ore, int Clay, int Obsidian, int Geode);
record struct Robots(int Ore, int Clay, int Obsidian, int Geode);