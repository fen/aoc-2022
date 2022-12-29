if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day18 <input>");
    return;
}

var cube = await Cube.CreateAsync(file);

Console.WriteLine("Part One: {0}", SolveOne(cube));
Console.WriteLine("Part Two: {0}", SolveTwo(cube));

static int SolveOne(Cube cube)
{
    return cube
        .Select(c => Point.Transforms
            .Select(t => t(c))
            .Where(m => !cube.Contains(m))
            .Select(_ => 1)
            .Aggregate(0, (acc, x) => acc + x)
        )
        .Aggregate(0, (acc, x) => acc + x);
}

static int SolveTwo(Cube cube)
{
    var water = FindWater(cube);
    return cube
        .Select(c => Point.Transforms
            .Select(t => t(c))
            .Where(m => water.Contains(m))
            .Select(_ => 1)
            .Aggregate(0, (acc, x) => acc + x)
        )
        .Aggregate(0, (acc, x) => acc + x);

    static HashSet<Point> FindWater(Cube c)
    {
        var (minX, maxX, minY, maxY, minZ, maxZ) = c;
        List<int> queue = new() { minX, minY, minZ };
        HashSet<Point> visited = new();
        HashSet<Point> water = new();
        while (queue.Count > 0) {
            if (queue is not [var x, var y, var z, ..]) throw new();
            queue.RemoveRange(0, 3);
            Point cube = new(x, y, z);
            if (visited.Contains(cube)) continue;
            visited.Add(cube);
            if (x < minX || x > maxX || y < minY || y > maxY || z < minZ || z > maxZ)
                continue;
            if (c.Contains(cube)) continue;
            water.Add(cube);
            foreach (var transform in Point.Transforms) {
                queue.AddRange(transform(cube));
            }
        }

        return water;
    }
}

record Cube(List<Point> Points, HashSet<Point> Lookup) : IEnumerable<Point>
{
    public static async Task<Cube> CreateAsync(string file)
    {
        var points = (await File.ReadAllLinesAsync(file))
            .Select(line => line.Split(',').Select(int.Parse).ToArray())
            .Select(n => new Point(n[0], n[1], n[2])).ToList();
        return new(points, new(points));
    }

    public void Deconstruct(out int minX, out int maxX, out int minY, out int maxY, out int minZ, out int maxZ)
    {
        minX = Points.Select(p => p.X).Min() - 1;
        maxX = Points.Select(p => p.X).Max() + 1;
        minY = Points.Select(p => p.Y).Min() - 1;
        maxY = Points.Select(p => p.Y).Max() + 1;
        minZ = Points.Select(p => p.Z).Min() - 1;
        maxZ = Points.Select(p => p.Z).Max() + 1;
    }

    public bool Contains(Point p) => Lookup.Contains(p);

    public IEnumerator<Point> GetEnumerator() => ((IEnumerable<Point>)Points).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Points).GetEnumerator();
}

record struct Point(int X, int Y, int Z) : IEnumerable<int>
{
    public static readonly Func<Point, Point>[] Transforms = {
        (p) => p with { Z = p.Z - 1 }, (p) => p with { Z = p.Z + 1 }, (p) => p with { X = p.X - 1 },
        (p) => p with { X = p.X + 1 }, (p) => p with { Y = p.Y - 1 }, (p) => p with { Y = p.Y + 1 },
    };

    public IEnumerator<int> GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"{X},{Y},{Z}";
}
