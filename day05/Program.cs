if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day5 <input>");
    return;
}

var (stacks, moves) = await ParseAsync(file);
var partTwoStacks = stacks.CloneMe();

foreach (var move in moves) {
    stacks.Execute(move);
}

foreach (var move in moves) {
    partTwoStacks.ExecuteImproved(move);
}

Console.WriteLine(string.Join("", stacks.Items.Where(i => i.Count > 0).Select(i => i.Peek())));
Console.WriteLine(string.Join("", partTwoStacks.Items.Where(i => i.Count > 0).Select(i => i.Peek())));

async Task<(Stacks, List<Move>)> ParseAsync(string file)
{
    var lines = await File.ReadAllLinesAsync(file);

    var stacks = Stacks.Parse(lines);

    var moves = new List<Move>();
    foreach (var moveLine in lines) {
        if (Move.TryParse(moveLine, out var move)) {
            moves.Add(move);
        }
    }

    return (stacks, moves);
}

record struct Move(int Count, int From, int To)
{
    static readonly Regex MoveRegex = new("move (\\d+) from (\\d+) to (\\d+)");

    public static bool TryParse(string line, out Move move)
    {
        if (MoveRegex.Match(line) is { Success: true, Groups: [_, var count, var from, var to] }) {
            move = new(int.Parse(count.Value), int.Parse(from.Value), int.Parse(to.Value));
            return true;
        }

        move = default;
        return false;
    }
}

record Stacks(List<Stack<char>> Items)
{
    public static Stacks Parse(string[] lines)
    {
        var stacks = new Stacks(new List<Stack<char>>());

        // We skip the footer and need to reverse because we parse top down
        // so reverse is need for the stack order to be correct
        var stackLines = lines.TakeWhile(line => line.Length > 0 && !line.Contains("1")).Reverse().ToArray();
        foreach (var stackLine in stackLines) {
            var stackLineSplit = stackLine.Split(' ');
            int stackCount = 0;
            for (int i = 0; i < stackLineSplit.Length; i++) {
                var entry = stackLineSplit[i];
                if (entry.Length == 0) {
                    i += 3; // the number of empty spaces an entry has
                    stacks.GetStack(stackCount);
                    stackCount += 1;
                    continue;
                }

                stacks.GetStack(stackCount)
                    .Push(entry.Substring(1, 1).First());
                stackCount += 1;
            }
        }
        return stacks;
    }

    public Stack<char> GetStack(int n)
    {
        if (Items.Count <= n) {
            var stack = new Stack<char>();
            Items.Add(stack);
            return stack;
        }

        return Items[n];
    }

    public void Execute(Move move)
    {
        var from = GetStack(move.From - 1);
        var to = GetStack(move.To - 1);
        for (int i = 0; i < move.Count; i++) {
            var moving = from.Pop();
            to.Push(moving);
        }
    }

    public void ExecuteImproved(Move move)
    {
        var from = GetStack(move.From - 1);
        var to = GetStack(move.To - 1);

        var placeholder = new List<char>();
        for (int i = 0; i < move.Count; i++) {
            var moving = from.Pop();
            placeholder.Add(moving);
        }

        placeholder.Reverse();
        foreach (var c in placeholder) {
            to.Push(c);
        }
    }

    public Stacks CloneMe()
    {
        var clone = new Stacks(new List<Stack<char>>());
        foreach (var item in Items) {
            clone.Items.Add(new Stack<char>(item.Reverse()));
        }
        return clone;
    }
}
