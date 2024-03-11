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
        var current = input.ToCharArray().Select(digit => digit - '0').ToList();

        for (var phase = 0; phase < phases; phase++)
        {
            var newCurrent = new List<int>(current.Count);

            for (var position = 0; position < current.Count; position++)
            {
                var result = 0;

                // We're always either 0s or 1ns.
                // So we should be able to reverse cumsum and go from there?
                foreach (var (index, number) in current.Enumerate())
                {
                    var locationInPattern = (index + 1) % (4 * (position + 1));
                    var locationInSubPattern = locationInPattern / (position + 1);
                    var appliedPatternValue = locationInSubPattern switch
                    {
                        0 => 0,
                        1 => 1,
                        2 => 0,
                        3 => -1,
                    };

                    result += number * appliedPatternValue;
                }

                newCurrent.Add(Math.Abs(result % 10));
            }

            current = newCurrent;
            //Console.WriteLine($"Phase {phase}: {string.Join("", current)}");
        }

        return string.Join("", current);
    }

    internal static string CalculatePhasesWithLargeSkip(string input, int phases, int repeats)
    {
        var skip = int.Parse(input[..7]);
        var current = input.ToCharArray().Select(digit => digit - '0').Repeat(repeats).Skip(skip).ToList();

        // The skip is large enough that all we have is a triangular matrix of ones, so we just need a reversed cumsum.
        for (var phase = 0; phase < phases; phase++)
        {
            current = current
                .Reversed()
                .CumulativeSum()
                .ToList()
                .Reversed()
                .Select(x => Math.Abs(x % 10))
                .ToList();
        }

        var result = string.Join("", current);
        return result[..8];
    }

    private Answer CalculatePart1Answer()
    {
        return CalculatePhases(_input, phases: 100)[..8];
    }

    private Answer CalculatePart2Answer()
    {
        return CalculatePhasesWithLargeSkip(_input, phases: 100, repeats: 10000)[..8];
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
