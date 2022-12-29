if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day16 <input>");
    return;
}

var input = await ParseInputAsync(file);

Console.WriteLine("Part One: {0}", Solve("AA", 30, 0, 0, new(), input).Values.Max());
Console.WriteLine("Part Two: {0}", PartTwo(input));

static int PartTwo(PreComputed p)
{
    var pairs = Solve("AA", 26, 0, 0, new(), p);

    int max = 0;
    foreach (var (k1, v1) in pairs) {
        foreach (var (k2, v2) in pairs) {
            if ((k1 & k2) == 0) {
                max = Math.Max(max, v1 + v2);
            }
        }
    }

    return max;
}

static Dictionary<int, int> Solve(string valve, int time, int bitmask, int flow, Dictionary<int, int> answer, PreComputed p)
{
    answer[bitmask] = answer.TryGetValue(bitmask, out var value) ? Math.Max(value, flow) : Math.Max(0, flow);

    foreach (var next in p.FlowRate.Keys) {
        var newBudget = time - p.Distances[valve][next] - 1;
        if ((p.BitMasks[next] & bitmask) != 0 || newBudget <= 0) {
            continue;
        }
        Solve(
            next, 
            newBudget, 
            bitmask | p.BitMasks[next],
            flow + newBudget * p.FlowRate[next],
            answer,
            p
        );
    }

    return answer;
}

static async Task<PreComputed> ParseInputAsync(string file)
{
    var r = new Regex("[\\s=;,]+");
    var lines = (await File.ReadAllLinesAsync(file))
        .Select(line => r.Split(line.Trim())).ToList();

    // Fastest is to precompute all of the lookup tables
    var graph = lines.Select(l => (l[1], l.AsSpan().Slice(10).ToArray()))
        .ToDictionary(i => i.Item1, i => i.Item2);
    var flowRates = lines.Where(l => l[5] != "0").Select(l => (l[1], int.Parse(l[5])))
        .ToDictionary(i => i.Item1, i => i.Item2); 
    var bitmasks = flowRates.Select((item, index) => (item.Key, 1 << index))
        .ToDictionary(i => i.Key, i => i.Item2);
    var distances = graph.Select(x => (
        x.Key, 
        graph.Select(y => (
                y.Key, 
                graph[x.Key].Contains(y.Key) ? 1 : int.MaxValue))
                        .ToDictionary(i => i.Key, i => i.Item2)
        )
    ).ToDictionary(i => i.Key, i => i.Item2);

    foreach (var k in distances.Keys) {
        foreach (var i in distances.Keys) {
            foreach (var j in distances.Keys) {
                distances[i][j] = Math.Min(distances[i][j], Add(distances[i][k], distances[k][j]));
            }
        }
    }

    return new(flowRates, bitmasks, distances);

    static int Add(int l, int r) => (l, r) switch {
        (int.MaxValue, _) => int.MaxValue,
        (_, int.MaxValue) => int.MaxValue,
        var (a, b) => a+b
    };
}

record PreComputed(
    Dictionary<string, int> FlowRate,
    Dictionary<string, int> BitMasks,
    Dictionary<string, Dictionary<string, int>> Distances
);

