if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day2 <input>");
    return;
}

var plays = await ParseAsync(file);

Console.WriteLine(
    "Total score part one: {0}",
    plays.Sum(CalculateScorePartOne)
);

Console.WriteLine(
    "Total score part two: {0}",
    plays.Sum(CalculateScorePartTwo)
);

static int CalculateScorePartOne(Play play)
{
    var playScore = play switch {
        var (op, me, _) when op == me => 3,
        (Shape.Scissors, Shape.Rock, _) => 6,
        (Shape.Paper, Shape.Scissors, _) => 6,
        (Shape.Rock, Shape.Paper, _) => 6,
        _ => 0
    };
    var shapeScore = play switch {
        (_, Shape.Rock, _) => 1,
        (_, Shape.Paper, _) => 2,
        (_, Shape.Scissors, _) => 3,
        _ => throw new Exception("Unknown Shape")
    };

    return playScore + shapeScore;
}

static int CalculateScorePartTwo(Play play)
{
    var shapeToPlay = play switch {
        (Shape.Paper, _, Outcome.Win) => Shape.Scissors,
        (Shape.Paper, _, Outcome.Lose) => Shape.Rock,
        (Shape.Rock, _, Outcome.Win) => Shape.Paper,
        (Shape.Rock, _, Outcome.Lose) => Shape.Scissors,
        (Shape.Scissors, _, Outcome.Win) => Shape.Rock,
        (Shape.Scissors, _, Outcome.Lose) => Shape.Paper,
        (var s, _, Outcome.Draw) => s,
        _ => throw new Exception("Unknown play shape")
    };

    return CalculateScorePartOne(play with { Me = shapeToPlay });
}

async Task<List<Play>> ParseAsync(string file)
{
    var lines = await File.ReadAllLinesAsync(file);

    List<Play> plays = new(lines.Length);
    foreach (var line in lines) {
        var (opponentShapeStr, meShapeStr) = line.Split(" ");

        var opponentShape = opponentShapeStr switch {
            "A" => Shape.Rock,
            "B" => Shape.Paper,
            "C" => Shape.Scissors,
            _ => throw new Exception("Unknown shape")

        };

        var meShape = meShapeStr switch {
            "X" => Shape.Rock,
            "Y" => Shape.Paper,
            "Z" => Shape.Scissors,
            _ => throw new Exception("Unknown shape")
        };

        var outcome = meShapeStr switch {
            "X" => Outcome.Lose,
            "Y" => Outcome.Draw,
            "Z" => Outcome.Win,
            _ => throw new Exception("Unknown shape")
        };

        plays.Add(new(opponentShape, meShape, outcome));
    }

    return plays;
}

readonly record struct Play(Shape Opponent, Shape Me, Outcome Outcome);

enum Shape : byte
{
    Rock,       // A or X
    Paper,      // B or Y
    Scissors    // C or Z
}

enum Outcome : byte
{
    Lose, // X
    Draw, // Y
    Win,  // Z
}
