if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day10 <input>");
    return;
}

var instructions = await ParseAsync(file);
{
    var display = new SegmentDisplay();
    var cpu = new Cpu(instructions, display);
    var refresh = new[] { 20, 60, 100, 140, 180, 220 };
    Console.WriteLine(
        "Part One: {0}", 
        cpu.Run(refresh).Zip(refresh).Sum(a => a.First * a.Second));
}

{
    var display = new SegmentDisplay();
    var cpu = new Cpu(instructions, display);
    var refresh = new[] { 40, 80, 120, 160, 200, 240};
    foreach (var _ in cpu.Run(refresh)) { }
    Console.WriteLine("Part Two:");
    display.Print();
}

async Task<List<Instruction>> ParseAsync(string file)
{
    var lines = await File.ReadAllLinesAsync(file);
    List<Instruction> instructions = new();
    foreach (var line in lines) {
        var (instruction, value) = line.Split(' ');
        instructions.Add(instruction switch {
            "addx" => new (2, int.Parse(value)),
            "noop" => new (1, 0),
            _ => throw new()
        });
    }

    return instructions;
}

class SegmentDisplay
{
    StringBuilder _drawLine = new();
    List<string> _lines = new();

    public int SpritePosition { get; set; }

    public void Print() => _lines.ForEach(Console.WriteLine);

    public void Draw(int count)
    {
        while(count-- > 0) {
            var x = _drawLine.Length+1;
            bool spriteHit = x >= SpritePosition && x <= SpritePosition + 2;
            if (spriteHit) _drawLine.Append("█");
            else _drawLine.Append(" ");
        }
    }

    public void Refresh()
    {
        _lines.Add(_drawLine.ToString());
        _drawLine.Clear();
    }
}

class Cpu
{
    readonly List<Instruction> _instructions;
    readonly SegmentDisplay _segmentDisplay;
    int? _instructionHaltCycles;

    public Cpu(List<Instruction> instructions, SegmentDisplay segmentDisplay)
    {
        _instructions = instructions;
        _segmentDisplay = segmentDisplay;
    }

    public int X { get; set; } = 1;
    public int Cycle { get; set; }
    public int InstrucitonPointer { get; set; }

    public IEnumerable<int> Run(int[] halts)
    {
        foreach (var halt in halts) {
            Continue(halt);
            _segmentDisplay.Refresh();
            yield return X;
        }
    }

    public void Continue(int halt)
    {
        for (; InstrucitonPointer < _instructions.Count; InstrucitonPointer++) {
            var instruction = _instructions[InstrucitonPointer];
            if (Step(instruction.Cycles, halt)) {
                return;
            }
            _segmentDisplay.SpritePosition = X += instruction.X;
        }
    }

    /// <summary>Step the cycles returns true if we are halting.</summary>
    bool Step(int increment, int halt)
    {
        // We just halted during an instruction could have cycles to
        // execute for this instruction
        if (_instructionHaltCycles.HasValue) {
            _segmentDisplay.Draw(_instructionHaltCycles.Value);

            Cycle += _instructionHaltCycles.Value;
            _instructionHaltCycles = null;

            return false;
        }

        var nextCycle = Cycle + increment;

        if (nextCycle >= halt) {
            _segmentDisplay.Draw(halt - Cycle); // draw the line
            _instructionHaltCycles = nextCycle - halt; // still to be executed
            Cycle = halt;
            return true;
        }

        _segmentDisplay.Draw(increment);
        Cycle = nextCycle;
        return false;
    }
}

record struct Instruction(int Cycles, int X);
