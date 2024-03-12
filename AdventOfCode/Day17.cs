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

    private void PrintMap(HashSet<(int Row, int Column)> map, (int Row, int Column) robot)
    {
        var maxRow = map.Max(x => x.Row);
        var maxColumn = map.Max(x => x.Column);

        for (var row = 0; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(0, maxColumn + 1)
                .Select(column => (row, column) == robot
                    ? 'R'
                    : map.Contains((row, column)) ? '#' : '.'
                )
                .ToList();

            Console.WriteLine(string.Join("", toPrint));
        }

        Console.WriteLine();
    }

    private (HashSet<(int Row, int Column)> Map, (int Row, int Column) Robot) GetMap()
    {
        var computerInputs = new List<long>();
        var computer = new Computer(_input, computerInputs);

        var map = new HashSet<(int Row, int Column)>();
        var robot = (Row: 0, Column: 0);

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
                    robot = (row, column);
                    column++;
                    break;
            }
        }
    }

    private Answer CalculatePart1Answer()
    {
        var (map, robot) = GetMap();
        PrintMap(map, robot);

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
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
