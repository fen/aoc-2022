if (args is not [var file] || !System.IO.File.Exists(file)) {
    Console.WriteLine("day8 <input>");
    return;
}

var grid = await Grid.ParseAsync(file);

var edges = grid.RowCount * 2 + grid.ColumnCount * 2;
Console.WriteLine("Part One: {0}", grid.GetCells().Count(c => c.IsVisible()) + edges);
Console.WriteLine("Part Two: {0}", grid.GetCells().Max(c => c.GetScenicScore()));

class Grid
{
    private List<Tree> Cells { get; } = new();

    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }

    private void Add(Tree tree)
    {
        RowCount = Math.Max(RowCount, tree.Row);
        ColumnCount = Math.Max(ColumnCount, tree.Column);
        Cells.Add(tree);
    }

    public Tree GetCell(int row, int column) => Cells[(row * (RowCount + 1)) + column];

    public IEnumerable<Tree> GetCells()
    {
        foreach (var cell in Cells) {
            if (cell.Row == 0 || cell.Column == 0 || cell.Row == RowCount || cell.Column == ColumnCount) {
                continue;
            }

            yield return cell;
        }
    }

    public static async Task<Grid> ParseAsync(string file)
    {
        var grid = new Grid();

        var rows = await File.ReadAllLinesAsync(file);
        List<Tree> cells = new();
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
        while (currentColumn >= 0 && currentColumn <= Grid.ColumnCount && currentRow >= 0 &&
               currentRow <= Grid.RowCount) {
            yield return Grid.GetCell(currentRow, currentColumn);
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