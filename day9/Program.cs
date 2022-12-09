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
    Console.WriteLine("Part One: {0}", simulator.Knots.Last().Recorded.Count);
}

void PartTwo()
{
    var simulator = new Simulator(10);
    simulator.Run(movements);
    Console.WriteLine("Part Two: {0}", simulator.Knots.Last().Recorded.Count);
}

static async Task<List<Movement>> ParseAsync(string file)
    => (await File.ReadAllLinesAsync(file)).Select(Movement.Parse).ToList();

class Simulator
{
    public Simulator(int numberOfKnots)
    {
        Knots = new List<Knot>(Enumerable.Range(0, numberOfKnots).Select(_ => new Knot()));
    }

    public List<Knot> Knots { get; }

    public void Run(List<Movement> movements) => movements.ForEach(Run);

    private void Run(Movement movements)
    {
        var head = Knots.First();
        foreach (var step in Steps(head.Position, movements)) {
            head.Position = step;

            for (var i = 1; i < Knots.Count; i++) {
                var current = Knots[i];
                var other = Knots[i - 1];
                if (!current.Position.IsNextTo(other.Position)) {
                    current.Position = current.Position.GetPosition(other.Position);
                }
            }
        }
    }

    private IEnumerable<Position> Steps(Position start, Movement movement)
    {
        Position p = movement.Direction switch {
            Direction.Left => new(-1, 0),
            Direction.Right => new(+1, 0),
            Direction.Up => new(0, +1),
            Direction.Down => new(0, -1),
            _ => throw new ArgumentOutOfRangeException()
        };

        var current = start;
        for (int i = 0; i < movement.Count; i++) {
            current += p;
            yield return current;
        }
    }
}

class Knot
{
    private Position _position;

    public Knot()
    {
        Recorded.Add(Position);
    }

    public Position Position {
        get => _position;
        set {
            _position = value;
            Recorded.Add(value);
        }
    }

    public HashSet<Position> Recorded { get; } = new();
}

record struct Position(int X, int Y)
{
    private static readonly Position[] Movements = {
        new(-1, 0),  // left
        new(+1, 0),  // right
        new(0, +1),  // up
        new(0, -1),  // down

        new(-1, -1), // bottom-left
        new(+1, -1), // bottom-right
        new(+1, +1), // top-right
        new(-1, +1), // top-left
    };

    public bool IsNextTo(Position other, bool isDirectlyNextTo = false)
    {
        if (Equals(other)) return true;
        foreach (var position in Movements.Take(isDirectlyNextTo ? 4 : 8)) {
            if (this + position == other) {
                return true;
            }
        }

        return false;
    }

    public Position GetPosition(Position other)
    {
        foreach (var position in Movements) {
            if ((this + position).IsNextTo(other, isDirectlyNextTo: true)) {
                return this + position;
            }
        }

        foreach (var position in Movements) {
            if ((this + position).IsNextTo(other, isDirectlyNextTo: false)) {
                return this + position;
            }
        }

        throw new();
    }

    public static Position operator +(Position left, Position right)
        => new(X: left.X + right.X, Y: left.Y + right.Y);
}

record Movement(Direction Direction, int Count)
{
    public static Movement Parse(string value)
    {
        var (direction, count) = value.Split(' ');

        return new(
            direction switch {
                "L" => Direction.Left,
                "R" => Direction.Right,
                "U" => Direction.Up,
                "D" => Direction.Down,
                _ => throw new("unknown direction")
            },
            int.Parse(count)
        );
    }
}

enum Direction
{
    Left,
    Right,
    Up,
    Down
}