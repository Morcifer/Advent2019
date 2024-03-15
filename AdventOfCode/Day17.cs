using System.Data.Common;
using System.Linq;
using Spectre.Console;

namespace AdventOfCode;

public sealed class Day17 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day17() : this(RunMode.Real)
    {
    }

    public Day17(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    private void PrintMap(HashSet<(int Row, int Column)> map, (int Row, int Column, char Orientation) robot)
    {
        var maxRow = map.Max(x => x.Row);
        var maxColumn = map.Max(x => x.Column);

        for (var row = 0; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(0, maxColumn + 1)
                .Select(column => (row, column) == (robot.Row, robot.Column)
                    ? robot.Orientation
                    : map.Contains((row, column)) ? '#' : '.'
                )
                .ToList();

            Console.WriteLine(string.Join("", toPrint));
        }

        Console.WriteLine();
    }

    private (HashSet<(int Row, int Column)> Map, (int Row, int Column, char Orientation) Robot) GetMap()
    {
        var computerInputs = new List<long>();
        var computer = new Computer(_input, computerInputs);

        var map = new HashSet<(int Row, int Column)>();
        var robot = (Row: 0, Column: 0, Orientation: 'R');

        var row = 0;
        var column = 0;

        while (true)
        {
            var (returnMode, result) = computer.RunProgram();

            if (returnMode == ReturnMode.Terminate)
            {
                return (map, robot);
            }

            if (returnMode == ReturnMode.Input)
            {
                return (map, robot);
            }

            switch (result.Value)
            {
                case 10: // "\n"
                    row++;
                    column = 0;
                    break;

                case 35: // "#"
                    map.Add((row, column));
                    column++;
                    break;

                case 46: // "."
                    column++;
                    break;

                default:
                    map.Add((row, column));
                    robot = (row, column, Convert.ToChar(result.Value));
                    column++;
                    break;
            }
        }
    }

    private Answer CalculatePart1Answer()
    {
        var (map, _) = GetMap();
        //PrintMap(map, robot);

        var deltas = new List<(int Row, int Column)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };
        var intersections = new List<(int Row, int Column)>();

        var maxRow = map.Max(x => x.Row);
        var maxColumn = map.Max(x => x.Column);

        for (var row = 0; row <= maxRow; row++)
        {
            for (var column = 0; column <= maxColumn; column++)
            {
                if (!map.Contains((row, column)))
                {
                    continue;
                }

                var neighbours = deltas
                    .Select(d => (row + d.Row, column + d.Column))
                    .Count(n => map.Contains(n));

                if (neighbours > 2)
                {
                    intersections.Add((row, column));
                }
            }
        }

        return intersections.Select(i => i.Row * i.Column).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        var (map, robot) = GetMap();
        PrintMap(map, robot);

        // R 6, L 10, R 10, R 10, R 10, L

        // Find sequence, hope that you don't have to turn at an intersection...
        var commandSequence = new List<(char Rotate, int Step)>();
        var simulatedRobot = (robot.Row, robot.Column, robot.Orientation);
        var deltas = new List<(int, int)> { (0, 1), (0, -1), (1, 0), (-1, 0) };
        var endOfTheRoad = map
            .Where(spot => 1 == deltas.Count(d => map.Contains((spot.Row + d.Item1, spot.Column + d.Item2))))
            .First(spot => spot != (robot.Row, robot.Column));


        while ((simulatedRobot.Row, simulatedRobot.Column) != endOfTheRoad && map.Contains((simulatedRobot.Row, simulatedRobot.Column)))
        {
            // Turn left or right.
            foreach (var turn in new List<char> { 'R', 'L', 'B' })
            {
                var newOrientation = (simulatedRobot.Orientation, turn) switch
                {
                    ('^', 'R') => '>',
                    ('^', 'L') => '<',
                    ('>', 'R') => 'v',
                    ('>', 'L') => '^',
                    ('v', 'R') => '<',
                    ('v', 'L') => '>',
                    ('<', 'R') => '^',
                    ('<', 'L') => 'v',
                };

                var newLocation = newOrientation switch
                {
                    '^' => (Row: simulatedRobot.Row - 1, simulatedRobot.Column),
                    'v' => (Row: simulatedRobot.Row + 1, simulatedRobot.Column),
                    '<' => (simulatedRobot.Row, Column: simulatedRobot.Column - 1),
                    '>' => (simulatedRobot.Row, Column: simulatedRobot.Column + 1),
                };

                // Found the right turn, now keep going until you're going to fall
                if (map.Contains(newLocation))
                {
                    var steps = 0;

                    while (map.Contains(newLocation))
                    {
                        steps++;

                        newLocation = newOrientation switch
                        {
                            '^' => newLocation with { Row = newLocation.Row - 1 },
                            'v' => newLocation with { Row = newLocation.Row + 1 },
                            '<' => newLocation with { Column = newLocation.Column - 1 },
                            '>' => newLocation with { Column = newLocation.Column + 1 },
                        };
                    }

                    commandSequence.Add((turn, steps));
                    //Console.WriteLine($"{turn} {steps}");

                    // Go to the final location before falling.
                    simulatedRobot = newOrientation switch
                    {
                        '^' => simulatedRobot with { Row = simulatedRobot.Row - steps, Orientation = newOrientation},
                        'v' => simulatedRobot with { Row = simulatedRobot.Row + steps, Orientation = newOrientation },
                        '<' => simulatedRobot with { Column = simulatedRobot.Column - steps, Orientation = newOrientation },
                        '>' => simulatedRobot with { Column = simulatedRobot.Column + steps, Orientation = newOrientation },
                    };

                    break;
                }
            }
        }

        var newProgram = _input.ToList();
        newProgram[0] = 2;

        var programToEncode = new List<string>()
        {
            "A,B,A,B,A,C,A,C,B,C",
            "R,6,L,10,R,10,R,10",
            "L,10,L,12,R,10",
            "R,6,L,12,L,10",
        };

        var computerInputs = new List<long>();

        foreach (var programLine in programToEncode)
        {
            computerInputs.AddRange(programLine.ToCharArray().Select(c => (long)c));
            computerInputs.Add(10); // Newline.
        }

        computerInputs.Add((long)'n'); // continuous video feed
        computerInputs.Add(10);

        var computer = new Computer(newProgram, computerInputs);

        var round = 0;
        var outputs = new List<long>();

        while (true)
        {
            round++;
            var (returnMode, result) = computer.RunProgram();

            if (returnMode == ReturnMode.Terminate)
            {
                break;
            }

            if (returnMode == ReturnMode.Input)
            {
                break;
            }

            if (result.Value != '.' && result.Value != '#' && result.Value != 10)
            {
                Console.WriteLine($"Output {round}: {result.Value}");
                outputs.Add(result.Value);
            }
            

            //switch (result.Value)
            //{
            //    continue;
            //}
        }

        var temp = outputs.Skip(1).Select(c => (char)c).ToList();
        Console.WriteLine(string.Join("", temp));
        return outputs[^1];
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
