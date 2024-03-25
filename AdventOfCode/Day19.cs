using Spectre.Console;

namespace AdventOfCode;

using GridSpot = (int Row, int Column);

public sealed class Day19 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day19() : this(RunMode.Real)
    {
    }

    public Day19(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    private Answer CalculatePart1Answer()
    {
        var known = new char[50, 50];

        var currentLocation = new GridSpot(0, 0);

        for (var row = 0; row < 50; row++)
        {
            for (var column = 0; column < 50; column++)
            {
                currentLocation = (row, column);

                var computerInputs = new List<long>();
                var computer = new Computer(_input, computerInputs);

                computerInputs.Add(currentLocation.Column);
                computerInputs.Add(currentLocation.Row);

                var results = computer.RunProgramToTermination();
                var result = results[^1];

                var found = result == 0 ? '.' : '#';
                known[currentLocation.Row, currentLocation.Column] = found;
                Console.WriteLine($"Grid spot {currentLocation} is {found}");
            }
        }

        var counter = 0;

        for (var i = 0; i < 50; i++)
        {
            var row = new List<char>();

            for (var j = 0; j < 50; j++)
            {
                if (known[i, j] == '#')
                {
                    counter++;
                }

                row.Add(known[i, j]);
            }

            Console.WriteLine($"{string.Join("", row)}");
        }

        return counter;
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
