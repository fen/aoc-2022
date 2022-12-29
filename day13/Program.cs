if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day13 <input>");
    return;
}

var input = await ReadAsync(file);

Console.WriteLine(
    "Part One: {0}",
    input.ByTwo()
        .Select(((p, i) => Comparer.Default.Compare(p.First, p.Second) < 0 ? i + 1 : 0))
        .Sum()
);

var first = List.New(List.New(2));
var second = List.New(List.New(6));
input.Add(first);
input.Add(second);
Console.WriteLine(
    "Part Two: {0}",
    input.Order(Comparer.Default)
        .Select((item, index) => (item, index))
        .Where(n => ReferenceEquals(n.item, first) || ReferenceEquals(n.item, second))
        .Select(n => n.index+1).Product()
);

static async Task<List<Item>> ReadAsync(string file)
{
    return (await File.ReadAllLinesAsync(file)).Where(l => l.Length > 0)
        .Select(p => {
            int index = 0;
            return Parse(p, ref index);
        }).ToList();

    static Item Parse(ReadOnlySpan<char> input, ref int index)
    {
        while (index < input.Length) {
            if (input[index] == '[') {
                index += 1;
                var currentList = new List(new());
                while (input[index] != ']') {
                    currentList.Items.Add(Parse(input, ref index));
                    if (input[index] == ',') index += 1; // skip ,
                }

                index += 1;
                return currentList;
            }

            if (char.IsNumber(input[index])) {
                int start = index++, count = 1;
                while (char.IsNumber(input[index])) {
                    index += 1;
                    count += 1;
                }

                return int.Parse(input.Slice(start, count));
            }
        }

        throw new();
    }
}

sealed class Comparer : IComparer<Item>
{
    public static readonly Comparer Default = new();

    public int Compare(Item? l, Item? r) => (l, r) switch {
        (Number nl, Number nr) => nl.Value.CompareTo(nr.Value),
        (List ll, Number nr) => Compare(ll, List.New(nr)),
        (Number nl, List lr) => Compare(List.New(nl), lr),
        (List { IsEmpty: true }, List { IsEmpty: true }) => 0,
        (List { IsEmpty: true }, List { IsEmpty: false }) => -1,
        (List { IsEmpty: false }, List { IsEmpty: true }) => 1,
        (List and [var a, .. var rl], List and [var b, .. var rr])
            => Compare(a, b) switch { 0 => Compare(rl, rr), var c => c },
        _ => throw new()
    };
}

abstract record Item
{
    public static implicit operator Item(int value) => new Number(value);
}

record Number(int Value) : Item
{
    public static implicit operator Number(int value) => new(value);
    public override string ToString() => Value.ToString();
}

record List(List<Item> Items) : Item
{
    public static List New(Item item) => new(new List<Item> { item });
    public int Count => Items.Count;
    public Item this[int index] => Items[index];
    public List Slice(int start, int length) => new(new List<Item>(Items.GetRange(start, length)));
    public bool IsEmpty => Items.Count == 0;
    public override string ToString() => $"[{string.Join(',', Items)}]";
}