if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day20 <input>");
    return;
}

Console.WriteLine("Part One: {0}", Solve(ParseInput(file, 1), 1));
Console.WriteLine("Part Two: {0}", Solve(ParseInput(file, 811589153), 10));

Item[] ParseInput(string file, Int128 n) 
    => File.ReadAllLines(file).Select(x => new Item(Int128.Parse(x)*n)).ToArray();

Int128 Solve(Item[] input, int mix)
{
    var previous = input.Pair().ToDictionary(x => x.Second, x => x.First);
    previous[input[0]] = input[^1];
    previous[input[^1]] = input[^2];

    var next = input.Pair().ToDictionary(x => x.First, x => x.Second);
    next[input[^2]] = input[^1];
    next[input[^1]] = input[0];

    Item? zeroNode = null;

    while (mix-- > 0) {
        foreach (var node in input) {
            if (node.Value == 0) {
                zeroNode = node;
                continue;
            }

            var (oldPrev, oldNext) = (previous[node], next[node]);
            previous[oldNext] = oldPrev;
            next[oldPrev] = oldNext;

            var newNext = oldNext;
            for (var i = 0; i < Int128.Abs(node.Value) % (input.Count() - 1); i++) {
                newNext = node.Value > 0 ? next[newNext] : previous[newNext];
            }
            var newPrev = previous[newNext];

            next[newPrev] = node;
            previous[node] = previous[newNext];
            next[node] = newNext;
            previous[newNext] = node;
        }
    }

    if (zeroNode is null) throw new();

    var currentNode = zeroNode;
    Int128 sum = 0;

    for (var i = 1; i <= 3000; i++) {
        currentNode = next[currentNode];
        if (i % 1000 == 0)  {
            sum += currentNode.Value;
        }
    }

    return sum;
}

record Item(Int128 Value) {
    public override int GetHashCode() => base.GetHashCode();
}