// if (args is not [var file] || !File.Exists(file)) {
//     Console.WriteLine("day19 <input>");
//     return;
// }
using System.Runtime.InteropServices;

var file = @"C:\src\prj\aoc-2022-take2\inputs\day_19.input";

var blueprints = await ReadAsync(file);
var maxgeodes = 0;
var totaltime = 24;

Console.WriteLine("Part One: {0}", part1());

static async Task<Blueprint[]> ReadAsync(string file)
{
    var blueprints = (await File.ReadAllLinesAsync(file)).Select((line, id) => {
        var d = line.Split(' ')
            .TakeAt(6, 12, 18, 21, 27, 30)
            .Select(int.Parse)
            .ToArray();

        return new Blueprint() {
            id = id + 1,
            cost = new int[][] {
                new int[] { d[0], 0, 0, 0 },
                new int[] { d[1], 0, 0, 0 },
                new int[] { d[2], d[3], 0, 0},
                new int[] { d[4], 0, d[5], 0 },
            },
        };
    }).ToList();

    const int maxint = 888888888; // easily identifiable in debug

    foreach (var bp in blueprints) {
        for (int mat = 0; mat < 4; mat++) {
            for (int r = 0; r < 4; r++) {
                if (bp.cost[r][mat] > bp.maxrobots[mat]) {
                    bp.maxrobots[mat] = bp.cost[r][mat];
                }
            }
            if (bp.maxrobots[mat] == 0) {
                bp.maxrobots[mat] = maxint;
            }
        }
    }

    return blueprints.ToArray();
}

int part1()
{
    State initState = new State() { robots = new int[] { 1, 0, 0, 0 } };
    long lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    long startTime = lastTime;
    int qls = 0;
    for (int id = 0; id < blueprints.Length; id++) {
        maxgeodes = 0;
        //VPf("Simulating Blueprint#%d for %ds: %v\n", id + 1, totaltime, blueprints[id]);
        run(id, totaltime, -1, initState);
        int ql = (id + 1) * maxgeodes;
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long duration = now - lastTime;
        lastTime = now;
        //VPf("BP#%2d => %2d geodes, %3d quality level, duration: %6.3f\n", id + 1, maxgeodes, ql, ((double)duration) / 1000);
        qls += ql;
    }
    Console.WriteLine("Total duration: {0:F3}", ((double)DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime) / 1000);
    return qls;
}

void run(int id, int time, int fact, State s)
{
    const int ore = 0; // abbrev in code: o
    const int clay = 1; // abbrev in code: c
    const int obsidian = 2; // abbrev in code: b
    const int geode = 3; // abbrev in code: g

    // run mining robots
    for (int mat = 0; mat < 4; mat++) {
        s.mats[mat] += s.robots[mat];
    }
    // run the factory to finish building its started robot, if any
    if (fact >= 0) {
        for (int mat = 0; mat < 4; mat++) { // consume building costs
            s.mats[mat] -= blueprints[id].cost[fact][mat];
        }
        s.robots[fact]++;
    }
    time--; // ok, consumed our time
            
    // out of time, cannot start a new cycle, stop successfully here.
    if (time <= 0) {
        if (s.mats[geode] > maxgeodes) {
            maxgeodes = s.mats[geode];
        }
        return;
    }
    // if we could not possibly beat the current max, abort this run
    int rg = s.robots[3];
    int mg = s.mats[3];
    for (int t = 0; t < time; t++) { // suppose we from now build a geode robot per turn
        mg += rg;
        rg++;
    }
    if (mg <= maxgeodes) {
        return;
    }

    // Now explore each possible factory action:
    // we do not limit to just the next step(second), we pursue the goal

    // first, no robots: how many geodes if we just create nothing until end
    int newgeodes = s.mats[geode] + (time * s.robots[geode]);
    if (newgeodes > maxgeodes) {
        maxgeodes = newgeodes;
    }

    // then DFS-explore the 4 branches where we decide to do a robot
    // in reverse as we try to build the "better" robots first
    for (int r = 3; r >= 0; r--) {
        // first check we are not already at max capacity for this type
        if (s.robots[r] >= blueprints[id].maxrobots[r]) {
            continue;
        }
        // how much steps we will have enough of all mats to build this robot?
        bool continueNextRobot = false;
        int wait = 0;
        for (int mat = 0; mat < 4; mat++) {
            // currently missing
            if (blueprints[id].cost[r][mat] > s.mats[mat]) {
                // no bot -> never, abort!
                if (s.robots[mat] == 0) {
                    continueNextRobot = true;
                    break;
                }
                // + s.robots[mat]-1 ==> round up to get time needed
                int mwait = (blueprints[id].cost[r][mat] - s.mats[mat] + s.robots[mat] - 1) / s.robots[mat];
                if (mwait > wait) {
                    if (mwait >= time) {
                        continueNextRobot = true;
                        break;
                    }
                    wait = mwait;
                }
            }
        }

        if (continueNextRobot) {
            continue;
        }

        if (wait > 0) {
            State s2 = new State() {
                mats = new int[] { 
                    s.mats[ore] + (wait * s.robots[ore]), 
                    s.mats[clay] + (wait * s.robots[clay]), 
                    s.mats[obsidian] + (wait * s.robots[obsidian]), 
                    s.mats[geode] + (wait * s.robots[geode]) 
                },
                robots = s.robots.ToArray()
            };
            run(id, time - wait, r, s2);
        } else {
            run(id, time, r, s);
        }
    }
    return;
}

[StructLayout(LayoutKind.Explicit, Size = 4 + (16*4) + 4*4)]
public struct Blueprint
{
    [FieldOffset(0)]
    public int id;

    [FieldOffset(4)]
    public fixed int cost[4][4]; // cost[i][j] = cost in material j to make robot i

    [FieldOffset(4 + (16*4))]
    public int[] maxrobots; // optim: max robots for the factory capacity
}

// whate defines the state of the system?
// we do not include time and the robot being made, as they are more transient
[StructLayout(LayoutKind.Explicit, Size = 4*4 + 4*4)]
public struct State
{
    [FieldOffset(0)]
    public int[] mats; // the materials in stock

    [FieldOffset(4*4)]
    public int[] robots; // the fleet of active robots
}

// record struct Skip(bool Ore, bool Clay, bool Obsidian);
// record struct Blueprint(Cost Ore, Cost Clay, Cost Obsidian, Cost Geode);
// record struct Cost(int Ore, int Clay, int Obsidian);
// record struct Resources(int Ore, int Clay, int Obsidian, int Geode);
// record struct Robots(int Ore, int Clay, int Obsidian, int Geode);

