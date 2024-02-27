using MathNet.Numerics;

namespace AdventOfCode;

public sealed class Day10 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day10() : this(RunMode.Real)
    {
    }

    public Day10(RunMode runMode)
    {
        RunMode = runMode;

        // We can probably do better by using a sparse representation, but it looks like it's unnecessary.
        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private int VisibleAsteroids(int monitoringRow, int monitoringColumn)
    {
        var directions = new HashSet<(int, int)>();

        for (var rowIndex = 0; rowIndex < _input.Count; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < _input[0].Length; columnIndex++)
            {
                if (_input[rowIndex][columnIndex] != '#')
                {
                    continue;
                }

                var deltaRow = monitoringRow - rowIndex;
                var deltaColumn = monitoringColumn - columnIndex;

                var greatestCommonDivisor = Euclid.GreatestCommonDivisor(deltaRow, deltaColumn);

                if (greatestCommonDivisor == 0)
                {
                    directions.Add((deltaRow, deltaColumn));
                }
                else
                {
                    var key = ((int)(deltaRow / greatestCommonDivisor), (int)(deltaColumn / greatestCommonDivisor));
                    directions.Add(key);
                }
            }
        }

        return directions.Count - 1;
    }

    private Answer CalculatePart1Answer()
    {
        var max = 0;

        for (var rowIndex = 0; rowIndex < _input.Count; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < _input[0].Length; columnIndex++)
            {
                if (_input[rowIndex][columnIndex] != '#')
                {
                    continue;
                }

                max = Math.Max(max, VisibleAsteroids(rowIndex, columnIndex));
            }
        }

        return max;
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
