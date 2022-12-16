if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day15 <input>");
    return;
}

List<Reading> readings = await ReadAsync(file);

bool isTest = file.Contains("_test");

Console.WriteLine(SolvePartOne(readings, isTest));
Console.WriteLine(SolvePartTwo(readings));

static int SolvePartOne(List<Reading> readings, bool isTest)
{
    var y = isTest ? 10 : 2_000_000;

    var rects = readings.Select(r => r.ToRect());
    var l = rects.Select(r => r.Left).Min();
    var r = rects.Select(r => r.Right).Max();

    var result = 0;
    for (int x = l; x <= r; x++) {
        Point p = (x, y);
        if (readings.Any(r => r.Beacon != p && r.InRange(p))) {
            result += 1;
        }
    }

    return result;
}

static long SolvePartTwo(List<Reading> readings)
{
    const int Size = 4_000_001;
    var r = Find(readings, new(0, 0, Size, Size))
        .First();
    return r.X * 4_000_000L + r.Y;

    static IEnumerable<Rect> Find(List<Reading> readings, Rect rect)
    {
        if (rect is { Width: 0, Height: 0 }) yield break;
        if (readings.Any(r => r.InRange(rect))) yield break;
        if (rect is { Width: 1, Height: 1 }) {
            yield return rect;
            yield break;
        }

        // Each rectangle is split into four children that we then search
        var (a, b, c, d) = rect.Children();
        foreach (var r in Find(readings, a)) yield return r;
        foreach (var r in Find(readings, b)) yield return r;
        foreach (var r in Find(readings, c)) yield return r;
        foreach (var r in Find(readings, d)) yield return r;
    }
}

static async Task<List<Reading>> ReadAsync(string file)
{
    var regex = new Regex(
        "Sensor at x=(?<sensorX>-?\\d+), y=(?<sensorY>-?\\d+): closest beacon is at x=(?<beaconX>-?\\d+), y=(?<becaonY>-?\\d+)");
    var text = await File.ReadAllTextAsync(file);
    List<Reading> items = new();
    foreach (Match match in regex.Matches(text)) {
        if (match is { Success: true, Groups: [_, var sx, var sy, var bx, var by] }) {
            items.Add(new(new(sx.Value, sy.Value), new(bx.Value, by.Value)));
        }
    }

    return items;
}

record struct Reading(Point Sensor, Point Beacon)
{
    public int Radius = ManhattanDistance(Sensor, Beacon);
    public bool InRange(Point pos) => ManhattanDistance(pos, Sensor) <= Radius;

    public bool InRange(Rect r)
    {
        var (topLeft, topRight, bottomRight, bottomLeft) = r.Corners;
        return InRange(topLeft) && InRange(topRight) && InRange(bottomLeft) && InRange(bottomRight);
    }

    public Rect ToRect() => new(Sensor.X - Radius, Sensor.Y - Radius, 2 * Radius + 1, 2 * Radius + 1);
    static int ManhattanDistance(Point p1, Point p2) => Abs(p1.X - p2.X) + Abs(p1.Y - p2.Y);
}

record struct Rect(int X, int Y, int Width, int Height)
{
    public int Left => X;
    public int Right => X + Width - 1;
    public int Top => Y;
    public int Bottom => Y + Height - 1;

    public (Point TopLeft, Point TopRight, Point BottomRight, Point BottomLeft) Corners =>
    (
        new(Left, Top),
        new(Right, Top),
        new(Right, Bottom),
        new(Left, Bottom)
    );

    public (Rect, Rect, Rect, Rect) Children()
    {
        var w0 = Width / 2;
        var w1 = Width - w0;
        var h0 = Height / 2;
        var h1 = Height - h0;
        return (
            new Rect(Left, Top, w0, h0),
            new Rect(Left + w0, Top, w1, h0),
            new Rect(Left, Top + h0, w0, h1),
            new Rect(Left + w0, Top + h0, w1, h1)
        );
    }
}