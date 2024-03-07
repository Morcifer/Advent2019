using MoreLinq.Extensions;

namespace AdventOfCode;

public sealed class Day16 : BaseTestableDay
{
    private readonly string _input;

    public Day16() : this(RunMode.Real)
    {
    }

    public Day16(RunMode runMode)
    {
        RunMode = runMode;

        _input = File.ReadAllLines(InputFilePath).First();
    }

    internal static string CalculatePhases(string input, int phases)
    {
        var basePattern = new List<int> { 0, 1, 0, -1 };

        var current = input;

        for (var phase = 0; phase < phases; phase++)
        {
            var newCurrent = new List<int>();

            for (var position = 0; position < current.Length; position++)
            {
                var appliedPattern = basePattern.SelectMany(x => new List<int> { x }.Repeat(position + 1)).ToList();

                var result = 0;

                foreach (var (index, character) in current.Enumerate())
                {
                    result += (character - '0') * appliedPattern[(1 + index) % appliedPattern.Count];
                }

                newCurrent.Add(Math.Abs(result % 10));
            }

            current = string.Join("", newCurrent);
        }

        return current;
    }

    private Answer CalculatePart1Answer()
    {
        return CalculatePhases(_input, 100)[..8];
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
