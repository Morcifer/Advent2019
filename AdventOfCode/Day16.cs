using MoreLinq.Extensions;

namespace AdventOfCode;

public sealed class Day16 : BaseTestableDay
{
    private readonly string _input;

    public Day16() : this(RunMode.Test)
    {
    }

    public Day16(RunMode runMode)
    {
        RunMode = runMode;

        _input = File.ReadAllLines(InputFilePath).First();
    }

    internal static string CalculatePhases(string input, int phases, int repeats, int skip)
    {
        var current = input.ToCharArray().Select(digit => digit - '0').Repeat(repeats).Skip(skip).ToList();

        for (var phase = 0; phase < phases; phase++)
        {
            var newCurrent = new List<int>(current.Count);

            for (var position = skip; position < (skip + current.Count); position++)
            {
                var result = 0;

                foreach (var (index, number) in current.Enumerate())
                {
                    var locationInPattern = (index + 1 + skip) % (4 * (position + 1));
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

                if (newCurrent.Count % 100 == 0)
                {
                    Console.WriteLine($"Phase {phase}: {newCurrent.Count} found out of {current.Count}.");
                }
            }

            current = newCurrent;
            Console.WriteLine($"Phase {phase}: {string.Join("", current)}");
        }

        return string.Join("", current);
    }

    internal static string CalculatePhasesWithSkip(string input, int phases, int repeats)
    {
        var skip = int.Parse(input[..7]);
        return CalculatePhases(input, phases: phases, repeats: repeats, skip: skip)[..8];
    }

    private Answer CalculatePart1Answer()
    {
        return CalculatePhases(_input, phases: 100, repeats: 1, skip: 0)[..8];
    }

    private Answer CalculatePart2Answer()
    {
        return CalculatePhasesWithSkip(_input, phases: 100, repeats: 10000)[..8];
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
