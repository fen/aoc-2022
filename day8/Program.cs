if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day8 <input>");
    return;
}

var grid = await Grid.ParseAsync(file);

var edges = (grid.RowCount * 2 + grid.ColumnCount * 2)-4; // -4 so we don't double count the edges
Console.WriteLine("Part One: {0}", grid.GetTrees().Count(c => c.IsVisible()) + edges);
Console.WriteLine("Part Two: {0}", grid.GetTrees().Max(c => c.GetScenicScore()));

class Grid
{
    private List<Tree> Trees { get; } = new();

    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }

    private void Add(Tree tree)
    {
        RowCount = Math.Max(RowCount, tree.Row+1);
        ColumnCount = Math.Max(ColumnCount, tree.Column+1);
        Trees.Add(tree);
    }

    public Tree GetTree(int row, int column) => Trees[row * RowCount + column];

    public IEnumerable<Tree> GetTrees()
    {
        foreach (var cell in Trees) {
            if (IsEdge(cell)) {
                continue;
            }
            yield return cell;
        }

        bool IsEdge(Tree tree) => tree.Row == 0 || tree.Row == RowCount - 1 || tree.Column == 0 ||
                                  tree.Column == ColumnCount - 1;
    }

    public static async Task<Grid> ParseAsync(string file)
    {
        var grid = new Grid();
        var rows = await File.ReadAllLinesAsync(file);
        for (var rowNumber = 0; rowNumber < rows.Length; rowNumber++) {
            var row = rows[rowNumber];
            for (var columnNumber = 0; columnNumber < row.Length; columnNumber++) {
                grid.Add(new(int.Parse($"{row[columnNumber]}"), rowNumber, columnNumber, grid));
            }
        }

        return grid;
    }
}

record Tree(int Value, int Row, int Column, Grid Grid)
{
    public bool IsVisible()
    {
        return Navigate(0, -1).All(IsVisible) ||
               Navigate(0, +1).All(IsVisible) ||
               Navigate(-1, 0).All(IsVisible) ||
               Navigate(+1, 0).All(IsVisible);
    }

    public int GetScenicScore()
    {
        return Count(Navigate(-1, 0)) *
               Count(Navigate(0, -1)) *
               Count(Navigate(+1, 0)) *
               Count(Navigate(0, +1))
            ;
    }

    IEnumerable<Tree> Navigate(int row, int column)
    {
        var currentRow = Row + row;
        var currentColumn = Column + column;
        while (currentColumn >= 0 && currentColumn < Grid.ColumnCount && currentRow >= 0 &&
               currentRow < Grid.RowCount) {
            yield return Grid.GetTree(currentRow, currentColumn);
            currentRow += row;
            currentColumn += column;
        }
    }

    bool IsVisible(Tree tree) => Value > tree.Value;

    int Count(IEnumerable<Tree> cells)
    {
        int count = 0;
        foreach (var cell in cells) {
            count += 1;
            if (cell.Value >= Value) {
                break;
            }
        }

        return count;
    }
}