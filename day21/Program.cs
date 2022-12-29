if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day21 <input>");
    return;
}

var jobs = new Jobs(File.ReadAllLines(file).Select(line => line.Split(':', ' ')).Select(Job.Create).ToDictionary(j => j.Id, j=> j));
Console.WriteLine("Part One: {0}", jobs.Get("root").GetValue(jobs));

var (humn, root) = jobs.Correction();

Int128 lo = 1000000000000;
Int128 hi = 10000000000000;
while (true) {
    Int128 mid = (lo + hi) / 2;
    humn.Set(mid);
    var c = root.Compare(jobs);
    if (c == 0) {
        break;
    } else if (c == 1) {
        lo = mid;
    } else if (c == -1) {
        hi = mid;
    }
}

Console.WriteLine("Part Two: {0}", humn.Get());

abstract record Job(string Id) {
    public static Job Create(string[] parts) {
        if (parts.Length == 3) {
            return new YellJob(parts[0], Int128.Parse(parts[2]));
        }
        return new WaitJob(parts[0], parts[2], parts[3], parts[4]);
    }

    public abstract Int128 GetValue(Jobs jobs);
}

record Jobs(Dictionary<string, Job> Items)
{
    public Job Get(string id) => Items[id];

    public (HumanJob, RootJob) Correction()
    {
        const string id = "humn", root = "root";
        var (_, left, _, right) = (WaitJob)Items[root];
        var newRoot = Items[root] = new RootJob(id, left, right);
        var n = Items[id] = new HumanJob(id);
        return ((HumanJob)n, (RootJob)newRoot);
    }
}

record YellJob(string Id, Int128 Value) : Job(Id) 
{
    public override Int128 GetValue(Jobs jobs) => Value;
}

record WaitJob(string Id, string Left, string Op, string Right) : Job(Id)
{
    Int128? _cached;
    uint _versionA, _versionB;
    uint _version = 0;

    public override Int128 GetValue(Jobs jobs)
    {
        var x = jobs.Get(Left).GetValue(jobs);
        var y = jobs.Get(Right).GetValue(jobs);
        return Op switch {
            "+" => x+y,
            "-" => x - y,
            "/" => x / y,
            "*" => x * y,
            _ => throw new(),
        };
    }
}

record RootJob(string Id, string Left, string Right) : Job(Id) 
{
    public override Int128 GetValue(Jobs jobs) => throw new();
    public int Compare(Jobs jobs) => jobs.Get(Left).GetValue(jobs).CompareTo(jobs.Get(Right).GetValue(jobs));
}

record HumanJob(string Id) : Job(Id) 
{
    private Int128 _value;

    public override Int128 GetValue(Jobs jobs) => _value;

    public void Set(Int128 value) => _value = value;
    public Int128 Get() => _value;
}