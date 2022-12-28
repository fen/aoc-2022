// var file = @"/home/fen/code/prj/aoc-2022/inputs/day_19_test.input";
var file = @"/home/fen/code/prj/aoc-2022/inputs/day_19.input";
var input = await ReadAsync(file);

//Console.WriteLine(input.Take(3).Select(BestGeodeCount).Product());
Console.WriteLine(SolveOne(input));
Console.WriteLine(SolveTwo(input));

int SolveOne(IEnumerable<int[,]> input)
{
    var sum = 0;

    input.Each((x, i) => {
        var geodes = BestGeodeCount(x, 24);
        sum += geodes*(i + 1);

        Console.WriteLine($"Blueprint {i + 1} - {geodes} geodes");
    });

    return sum;
}

int SolveTwo(IEnumerable<int[,]> input) =>
    input.Take(3).Select(i => BestGeodeCount(i, 32)).Product();

static async Task<IEnumerable<int[,]>> ReadAsync(string file)
{
    return (await File.ReadAllLinesAsync(file)).Select(line => {
        var d = line.Split(' ')
            .TakeAt(6, 12, 18, 21, 27, 30)
            .Select(int.Parse)
            .ToArray();

        return new[,] {
            { d[0], 0,               0               },
            { d[1], 0,               0               },
            { d[2], d[3],            0               },
            { d[4],                  0,         d[5] }
        };
    });
}

static int BestGeodeCount(int[,] costs, int t)
{
    var bestGeodeCount = 0;

    var maxOreCost = new[] { costs[0, 0], costs[1, 0], costs[2, 0], costs[3, 0] }.Max();

    void Recurse(int elapsed, int[] robots, int[] resources, Skip skip)
    {
        var nextResources = resources.Zip(robots, (x, y) => x + y).ToArray();

        if (elapsed == (t-1)) {
            if (nextResources[3] > bestGeodeCount) 
                bestGeodeCount = nextResources[3];
            return;
        }

        var remaining = t - elapsed;

        var canBuildOre = resources[0] >= costs[0, 0]
                        && resources[0] + remaining * robots[0] < remaining * maxOreCost;

        var canBuildClay = resources[0] >= costs[1, 0]
                        && resources[1] + remaining * robots[1] < remaining * costs[2, 1];

        var canBuildObsidian = resources[0] >= costs[2, 0]
                            && resources[1] >= costs[2, 1]
                            && resources[2] + remaining * robots[2] < remaining * costs[3, 2];

        var canBuildGeode = resources[0] >= costs[3, 0]
                            && resources[2] >= costs[3, 2];

        if (resources[0] <= maxOreCost) {
            Recurse(
                elapsed + 1, 
                robots, 
                nextResources, 
                new(canBuildOre, canBuildClay, canBuildObsidian)
            );
        }

        if (canBuildOre && !skip.Ore) {
            Recurse(
                elapsed + 1,
                new[] { robots[0] + 1, robots[1], robots[2], robots[3] },
                new[] { nextResources[0] - costs[0, 0], nextResources[1], nextResources[2], nextResources[3] },
                default
            );
        }
        if (canBuildClay && !skip.Clay) {
            Recurse(
                elapsed + 1,
                new[] { robots[0], robots[1] + 1, robots[2], robots[3] },
                new[] { nextResources[0] - costs[1, 0], nextResources[1], nextResources[2], nextResources[3] },
                default
            );
        }
        if (canBuildObsidian && !skip.Obsidian) {
            Recurse(
                elapsed + 1,
                new[] { robots[0], robots[1], robots[2] + 1, robots[3] },
                new[] { nextResources[0] - costs[2, 0], nextResources[1] - costs[2, 1], nextResources[2], nextResources[3] },
                default
            );
        }
        if (canBuildGeode) {
            Recurse(
                elapsed + 1,
                new[] { robots[0], robots[1], robots[2], robots[3] + 1 },
                new[] { nextResources[0] - costs[3, 0], nextResources[1], nextResources[2] - costs[3, 2], nextResources[3] },
                default
            );
        }
    }

    Recurse(0, new[] { 1, 0, 0, 0 }, new[] { 0, 0, 0, 0 }, default);

    return bestGeodeCount;
}

record struct Skip(bool Ore, bool Clay, bool Obsidian);