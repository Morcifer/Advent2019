namespace AdventOfCode;

public sealed class Day17 : BaseTestableDay
{
    private readonly Computer _computer;
    private readonly List<long> _computerInputs;

    private (int Row, int Column, long Direction) _robot;
    private HashSet<(int Row, int Column)> _map;

    public Day17() : this(RunMode.Real)
    {
    }

    public Day17(RunMode runMode)
    {
        RunMode = runMode;

        var program = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();

        _computerInputs = new List<long>();
        _computer = new Computer(program, _computerInputs);

        _robot = (Row: 0, Column: 0, ' ');
        _map = new HashSet<(int Row, int Column)>();
    }

    private void PrintMap()
    {
        var minRow = _map.Min(x => x.Row);
        var maxRow = _map.Max(x => x.Row);
        var minColumn = _map.Min(x => x.Column);
        var maxColumn = _map.Max(x => x.Column);

        for (var row = minRow; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(minColumn, maxColumn - minColumn + 1)
                .Select(column => (row, column) == (_robot.Row, _robot.Column)
                    ? 'R'
                    : _map.Contains((row, column)) ? '#' : '.'
                )
                .ToList();

            Console.WriteLine(string.Join("", toPrint));
        }

        Console.WriteLine();
    }

    private void UpdateMap()
    {
        var row = 0;
        var column = 0;

        while (true)
        {
            var (returnMode, result) = _computer.RunProgram();

            if (returnMode == ReturnMode.Terminate)
            {
                return;
            }

            if (returnMode == ReturnMode.Input)
            {
                return;
            }

            switch (result.Value)
            {
                case 10: // "\n"
                    row++;
                    column = 0;
                    break;

                case 35: // "#"
                    _map.Add((row, column));
                    column++;
                    break;

                case 46: // "."
                    column++;
                    break;

                default:
                    _map.Add((row, column));
                    _robot = (row, column, result.Value);
                    column++;
                    break;
            }
        }
    }

    private Answer CalculatePart1Answer()
    {
        UpdateMap();
        PrintMap();

        var deltas = new List<(int Row, int Column)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };
        var intersections = new List<(int Row, int Column)>();

        var maxRow = _map.Max(x => x.Row);
        var maxColumn = _map.Max(x => x.Column);

        for (var row = 0; row < maxRow; row++)
        {
            for (var column = 0; column < maxColumn; column++)
            {
                if (!_map.Contains((row, column)))
                {
                    continue;
                }

                var neighbours = deltas
                    .Select(d => (row + d.Row, column + d.Column))
                    .Count(n => _map.Contains(n));

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
