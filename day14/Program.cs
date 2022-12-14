if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day14 <input>");
    return;
}

var grid = await Grid.ReadAsync(file);

var partTwoGrid = grid.Clone();
Console.WriteLine("Part One: {0}", Solve(grid, 1));
Console.WriteLine("Part Two: {0}", Solve(partTwoGrid, 2));

static int Solve(Grid grid, int part)
{
    var directions = new Point[] { new(0, 1), new(-1, 1), new(1, 1) };

    var maxY = grid.Select(p => p.Y).Max();

    if (part == 2) {
        var max = maxY + 2; // two plus the highest y coordinate
        var maxX = grid.Select(p => p.X).Max() * 2;
        for (int x = -maxX; x <= maxX; x++) {
            grid[new(x,max)] = '#';
        }
    }

    int count = 0;
    bool done = false;
    while (!done) {
        Point current = new(500, 0);

        if (part == 2 && grid.Contains(current)) {
            break;
        }

        while (true) {
            if (part == 1 && current.Y > maxY) {
                done = true;
                break;
            }

            Point? next = null;
            foreach (var direction in directions) {
                if (!grid.Contains((current + direction))) {
                    next = current + direction;
                    break;
                }
            }

            if (next == null) {
                grid[current] = 'o';
                count += 1;
                break;
            }

            current = next.Value;
        }

    }

    return count;
}

class Grid : IEnumerable<Point>
{
    readonly Dictionary<Point, char> _items = new();
    public Grid() => _items = new();
    Grid(Dictionary<Point, char> items) => _items = items;

    public bool Contains(Point p) => _items.ContainsKey(p);

    public char this[Point p] {
        get {
            throw new();
        }
        set {
            _items[p] = value;
        }
    }

    public Grid Clone() => new(new(_items));

    public static async Task<Grid> ReadAsync(string file)
    {
        var lines = await File.ReadAllLinesAsync(file);

        var grid = new Grid();
        foreach (var line in lines) {
            line.Split(" -> ")
                .SelectMany(l => l.Split(','))
                .Select(n => int.Parse(n))
                .ByTwo()
                .Pair()
                .Each((((int, int), (int, int)) p) => {
                    Point p1 = p.Item1, p2 = p.Item2;
                    var d = p2 - p1;
                    d /= d.Abs();
                    while (true) {
                        grid[p1] = '#';
                        if (p1 == p2) break;
                        p1 += d;
                    }
                });
        }
        return grid;
    }

    public IEnumerator<Point> GetEnumerator() => _items.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

record struct Point(int X, int Y)
{
    public static readonly Point NAP = new(int.MinValue, int.MinValue);

    public static implicit operator Point((int, int) p) => new(p.Item1, p.Item2);

    public static Point operator +(Point left, Point right)
        => new(X: left.X + right.X, Y: left.Y + right.Y);

    public static Point operator -(Point left, Point right)
        => new(X: left.X - right.X, Y: left.Y - right.Y);

    public static Point operator /(Point left, Point right)
        => new(X: left.X / Math.Max(right.X, 1), Y: left.Y / Math.Max(right.Y, 1));

    public Point Abs() => new(Math.Abs(X), Math.Abs(Y));
}
