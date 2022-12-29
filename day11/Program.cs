if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day11 <input>");
    return;
}

{
    var notes = await ParseAsync(file);
    Console.WriteLine("PartOne: {0}", Count(notes, 20, true).OrderDescending().Take(2).Product());
}
{
    var notes = await ParseAsync(file);
    Console.WriteLine("PartOne: {0}", Count(notes, 10_000, false).OrderDescending().Take(2).Product());
}

static async Task<List<Monkey>> ParseAsync(string file)
{
    var source = await File.ReadAllTextAsync(file);
    var regex = new Regex("""
Monkey (?<monkey>\d+):
  Starting items: (?<startingItems>[\d+][,\s*\d+)]*)
  Operation: new = (?<left>[\w|\d]+) (?<op>\W) (?<right>[\w|\d)]+)
  Test: divisible by (?<divisibleBy>\d+)
    If true: throw to monkey (?<ifTrue>\d+)
    If false: throw to monkey (?<ifFalse>\d+)
""");

    List<Monkey> notes = new();
    foreach (Match match in regex.Matches(source)) {
        notes.Add(new(
            int.Parse(match.Groups["monkey"].Value),
            ExpressionBuilder(match.Groups["left"].Value, match.Groups["op"].Value, match.Groups["right"].Value),
            match.Groups["startingItems"].Value.Split(", ").Select(long.Parse).ToList(),
            long.Parse(match.Groups["divisibleBy"].Value),
            int.Parse(match.Groups["ifTrue"].Value),
            int.Parse(match.Groups["ifFalse"].Value)
        ));
    }

    return notes;

    static Func<long, long> ExpressionBuilder(string left, string op, string right)
    {
        ParameterExpression old = Expression.Parameter(typeof(long), "old");
        return Expression.Lambda<Func<long, long>>(
            CreateOperation(
                Create(left, old),
                op,
                Create(right, old)
            ), old).Compile();

        static Expression CreateOperation(Expression left, string op, Expression right) =>
            op switch {
                "+" => Expression.Add(left, right),
                "*" => Expression.Multiply(left, right),
                "-" => Expression.Multiply(left, right),
                _ => throw new(op)
            };

        static Expression Create(string expression, ParameterExpression old) =>
            expression switch {
                "old" => old,
                var e when long.TryParse(e, out var constant) => Expression.Constant(constant, typeof(long)),
                _ => throw new(),
            };
    }
}

static long[] Count(List<Monkey> monkeys, int rounds, bool worryLess)
{
    long modules = monkeys.Select(n => n.DivisibleBy).Product();
    var counts = new long[monkeys.Count];
    for (int round = 1; round <= rounds; round++) {
        foreach (var note in monkeys) {
            while (note.Items.TryRemoveFirst(out var item)) {
                counts[note.Index] += 1;
                var newWorryLevel = note.Operation(item);
                long bitLessWorryLevel = worryLess ? newWorryLevel / 3 : newWorryLevel % modules;
                monkeys[bitLessWorryLevel % note.DivisibleBy == 0 ? note.TrueThrowToMonkey : note.FalseThrowToMonkey].Items.Add(bitLessWorryLevel);
            }
        }
    }

    return counts;
}

record Monkey(int Index, Func<long, long> Operation, List<long> Items, long DivisibleBy,
    int TrueThrowToMonkey,
    int FalseThrowToMonkey);