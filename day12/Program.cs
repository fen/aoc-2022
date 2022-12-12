if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day12 <input>");
    return;
}

Console.WriteLine("Part One: {0}", await SolveAsync(file, partOne: true));
Console.WriteLine("Part Two: {0}", await SolveAsync(file, partOne: false));

static async Task<int> SolveAsync(string file, bool partOne)
{
    var grid = await Grid.CreateAsync(file);

    var (start, end) = grid.FindStartEnd();

    int dist = 0;
    var seen = new HashSet<Point> { end };
    List<Point> current = new() { end }, next = new();
    while (current.Count > 0) {
        foreach (var point in current.SelectMany(grid.Transitions).Where(p => !seen.Contains(p))) {
            switch (partOne) {
            case true when point == start:
                return dist + 1;
            case false when grid.GetHeight(point) is (_, 'a'):
                return dist + 1;
            default:
                seen.Add(point);
                next.Add(point);
                break;
            }
        }

        (current, next) = (next, current);
        next.Clear();
        dist += 1;
    }

    throw new();
}

record Grid(List<string> Lines)
{
    public IEnumerable<Point> Transitions(Point p)
    {
        var (_, startHeight) = GetHeight(p);
        foreach (var point in p.GetAdjacent()) {
            if (GetHeight(point) is (true, var height) && height-startHeight >= -1) {
                yield return point;
            }
        }
    }

    public (bool, char) GetHeight(Point p)
    {
        if (p.X < 0 || p.Y < 0 || p.X >= Lines[0].Length || p.Y >= Lines.Count)
            return (false, default);

        return Lines[p.Y][p.X] switch {
            'S' => (true, 'a'),
            'E' => (true, 'z'),
            var c => (true, c)
        };
    }

    public (Point, Point) FindStartEnd()
    {
        int y = 0, x;
        Point start = default, end = default;
        foreach (var line in Lines) {
            x = 0;
            foreach (var c in line) {
                if (c == 'S') start = new(x, y);
                if (c == 'E') end = new(x, y);
                x += 1;
            }
            y += 1;
        }

        return (start, end);
    }

    public static async Task<Grid> CreateAsync(string file)
        => new((await File.ReadAllLinesAsync(file)).Reverse().ToList());
}

record struct Point(int X, int Y)
{
    static readonly Point L = new(-1, 0), R = new(+1, 0), U = new(0, +1), D = new(0, -1);
    public Point[] GetAdjacent() => new[] { this + L, this + D, this + R, this + U, };
    public static Point operator +(Point left, Point right)
        => new(X: left.X + right.X, Y: left.Y + right.Y);
}