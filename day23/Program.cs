if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day23 <input>");
    return;
}

var elves = Read(file);
Console.WriteLine("Part One: {0}", SolveOne(elves));
Console.WriteLine("Part Two: {0}", Simulate(elves, 10, int.MaxValue));

static int SolveOne(ElvesMap elves)
{
    Simulate(elves, 0, 10);
    var bounds = GetBounds(elves);
    return (bounds.maxX - bounds.minX + 1) * (bounds.maxY - bounds.minY + 1) - elves.Count;
}

static ElvesMap Read(string file)
{
    ElvesMap elves = new(new());
    File.ReadAllLines(file)
        .Each((line, y) => line.Each((c, x) => { if (c == '#') elves.Add(new(x, y)); }));

    return elves;
}

static int Simulate(ElvesMap elves, int start, int max)
{
    ElvesMap newMap = new(new());
    Dictionary<Point, Point> used = new();

    while (start < max) {
        newMap.Clear(); used.Clear();
        int moved = 0;
        foreach (var p in elves) {
            var f = (HasElf(start, p), HasElf(start + 1, p), HasElf(start + 2, p), HasElf(start + 3, p));
            var (N, S, W, E) = f;

            if (!N.HasElf && !S.HasElf && !W.HasElf && !E.HasElf) {
                newMap.Add(p);
                continue;
            } else if (N.HasElf && S.HasElf && W.HasElf && E.HasElf) {
                newMap.Add(p);
                continue;
            }

            Point dir = f switch {
                ((false, _), _, _, _) => N.Dir,
                (_, (false,_), _, _) => S.Dir,
                (_, _, (false, _), _) => W.Dir,
                (_, _, _, (false, _)) => E.Dir,
                _ => throw new()
            };

            var n = p + dir;
            if (used.TryGetValue(n, out var orig)) {
                used[n] = Point.Max;
                if (orig != Point.Max) {
                    newMap.Remove(n);
                    newMap.Add(orig);
                    moved--;
                }
                newMap.Add(p);
                continue;
            }

            used[n] = p;
            newMap.Add(n);
            moved++;
        }

        (newMap, elves) = (elves, newMap);
        start++;
        if (moved == 0) return start;
    }

    return start;

    (bool HasElf, Point Dir) HasElf(int dir, Point p)
    {
        return (dir & 3) switch {
            0 => (elves.Ok(p + Point.LD) || elves.Ok(p + Point.D) || elves.Ok(p + Point.RD), Point.D),
            1 => (elves.Ok(p + Point.LU) || elves.Ok(p + Point.U) || elves.Ok(p + Point.RU), Point.U),
            2 => (elves.Ok(p + Point.LD) || elves.Ok(p + Point.L) || elves.Ok(p + Point.LU), Point.L),
            _ => (elves.Ok(p + Point.RD) || elves.Ok(p + Point.R) || elves.Ok(p + Point.RU), Point.R)
        };
    }
}

static (int minX, int minY, int maxX, int maxY) GetBounds(ElvesMap elves)
{
    int minX = int.MaxValue; int minY = int.MaxValue;
    int maxX = int.MinValue; int maxY = int.MinValue;
    foreach (var pos in elves) {
        if (pos.X < minX) { minX = pos.X; }
        if (pos.X > maxX) { maxX = pos.X; }
        if (pos.Y < minY) { minY = pos.Y; }
        if (pos.Y > maxY) { maxY = pos.Y; }
    }

    return (minX, minY, maxX, maxY);
}


record ElvesMap(HashSet<Point> Items) : IEnumerable<Point>
{
    public void Add(Point p) => Items.Add(p);
    public bool Ok(Point p) => Items.Contains(p);
    public void Clear() => Items.Clear();
    public void Remove(Point p) => Items.Remove(p);
    public int Count => Items.Count;
    public IEnumerator<Point> GetEnumerator() => ((IEnumerable<Point>)Items).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
}

record struct Point(int X, int Y)
{
    public static readonly Point Max = new(int.MaxValue, int.MaxValue);
    public static readonly Point L = new(-1, 0), R = new(+1, 0), U = new(0, +1), D = new(0, -1),
                                 LD = L + D, RD = R + D, LU = L + U, RU = R + U;
    public Point[] GetAdjacent() => new[] { this + L, this + D, this + R, this + U, };
    public static Point operator +(Point left, Point right)
        => new(X: left.X + right.X, Y: left.Y + right.Y);
}
