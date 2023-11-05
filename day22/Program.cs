if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day22 <input>");
    return;
}

Console.WriteLine(Solve(Parse(file)));

(char[][] map, int[] distances, char[] turns) Parse(string file)
{
    var parts = File.ReadAllText(file).Split("\n\n");

    var mapInput = parts[0].Split("\n");
    var commandInput = parts[1];

    var mapWidth = mapInput.Max(x => x.Length);
    var map = mapInput.Select(x => {
        var padding = Enumerable.Range(0, mapWidth - x.Length)
            .Select(_ => ' ')
            .ToArray();
        return x.Concat(padding).ToArray();
    }).ToArray();

    var distances = commandInput
        .Split('R', 'L')
        .Select(int.Parse)
        .ToArray();

    var turns = commandInput
        .Where(x => x == 'R' || x == 'L')
        .ToArray();

    return (map, distances, turns);
}

object Solve((char[][] map, int[] distances, char[] turns) input)
{
    var (map, distances, turns) = input;

    var direction = Direction.Right;

    (int x, int y) position = default;
    for (var i = 0; i < map[0].Length; i++) {
        if (map[0][i] == '.') {
            position = (i, 0);
            break;
        };
    }

    Direction Turn(char side) =>
        (Direction)(((int)direction + (side == 'L' ? 3 : 1)) % 4);

    position = Move(position, direction, distances[0], map);

    for (var i = 0; i < turns.Length; i++) {
        direction = Turn(turns[i]);
        position = Move(position, direction, distances[i + 1], map);
    }

    return 1000 * (position.y + 1) + 4 * (position.x + 1) + (int)direction;
}

(int x, int y) Move((int x, int y) from, Direction direction, int distance, char[][] map)
{
    var nextPosition = from;
    var newPosition = from;
    var steps = 0;

    while (true) {
        if (steps == distance) break;

        nextPosition = GetStepPosition(nextPosition, direction, map);
        var nextChar = map[nextPosition.y][nextPosition.x];

        if (nextChar == '#') break;

        if (nextChar == '.') {
            steps++;
            newPosition = nextPosition;
        }
    }

    return newPosition;
}

(int x, int y) GetStepPosition((int x, int y) from, Direction direction, char[][] map)
{
    var width = map[0].Length;
    var length = map.Length;

    (int x, int y) nextPosition = direction switch {
        Direction.Right => (from.x + 1, from.y),
        Direction.Down => (from.x, from.y + 1),
        Direction.Left => (from.x - 1, from.y),
        Direction.Up => (from.x, from.y - 1),
    };

    return (
        (nextPosition.x + width) % width,
        (nextPosition.y + length) % length
    );
}

enum Direction { Right, Down, Left, Up }