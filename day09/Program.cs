if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day9 <input>");
    return;
}

var movements = await ParseAsync(file);
PartOne();
PartTwo();

void PartOne()
{
    var simulator = new Simulator(2);
    simulator.Run(movements);
    Console.WriteLine("Part One: {0}", simulator.Recorded.Count);
}

void PartTwo()
{
    var simulator = new Simulator(10);
    simulator.Run(movements);
    Console.WriteLine("Part Two: {0}", simulator.Recorded.Count);
}

static async Task<List<Movement>> ParseAsync(string file)
    => (await File.ReadAllLinesAsync(file)).Select(Movement.Parse).ToList();

class Simulator
{
    Position[] _knots;

    public Simulator(int numberOfKnots) => _knots = new Position[numberOfKnots];
    public HashSet<Position> Recorded { get; } = new() { new() };

    public void Run(List<Movement> movements)
    {
        foreach (var movement in movements) {
            foreach (var step in Steps(_knots[0], movement)) {
                _knots[0] = step;

                var last = _knots.Length - 1;
                for (var i = 1; i < _knots.Length; i++) {
                    var newPosition = _knots[i].Move(_knots[i - 1]);
                    if (_knots[i] != newPosition) {
                        if (i == last) Recorded.Add(newPosition);
                        _knots[i] = newPosition;
                    }
                }
            }
        }
    }

    IEnumerable<Position> Steps(Position start, Movement movement)
    {
        for (int i = 0; i < movement.Count; i++)
            yield return start += movement.Position;
    }
}

record struct Position(int X, int Y)
{
    public Position Move(Position o)
    {
        var x = (o - this);
        var p = x.Abs();
        if (p.X > 1 || p.Y > 1)
            return this + x / p;
        return this;
    }

    public static Position operator +(Position left, Position right)
        => new(X: left.X + right.X, Y: left.Y + right.Y);
    public static Position operator -(Position left, Position right)
        => new(X: left.X - right.X, Y: left.Y - right.Y);

    public static Position operator /(Position left, Position right)
        => new(X: left.X / Math.Max(right.X, 1), Y: left.Y / Math.Max(right.Y, 1));

    public Position Abs() => new(Math.Abs(X), Math.Abs(Y));
}

record Movement(Position Position, int Count)
{
    public static Movement Parse(string value)
    {
        var (direction, count) = value.Split(' ');
        return new(
            direction switch {
                "L" => new(-1, 0),
                "R" => new(+1, 0),
                "U" => new(0, +1),
                "D" => new(0, -1),
                _ => throw new()
            },
            int.Parse(count)
        );
    }
}
