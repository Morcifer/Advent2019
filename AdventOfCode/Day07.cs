using MoreLinq.Extensions;

namespace AdventOfCode;

public sealed class Day07 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day07() : this(RunMode.Real)
    {
    }

    public Day07(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    internal static long CalculateAmplifierRun(List<long> program, List<long> thrusters, bool stopAfterOneGo)
    {
        long signal = 0;

        var inputs = thrusters.Select(i => new List<long>() { i }).ToList();
        var computers = thrusters.Select((_, index) => new Computer(program, inputs[index])).ToList();

        while (true)
        {
            for (var amplifier = 0; amplifier < 5; amplifier++)
            {
                inputs[amplifier].Add(signal);

                var (returnMode, output) = computers[amplifier].RunProgram();

                if (returnMode == ReturnMode.Terminate)
                {
                    return signal;
                }

                signal = output.Value;
            }

            if (stopAfterOneGo)
            {
                return signal;
            }
        }
    }

    private Answer CalculatePart1Answer()
    {
        long max = 0;

        var phases = Enumerable.Range(0, 5).Select(x => (long)x).ToList();

        foreach (var phasePermutation in phases.Permutations())
        {
            var result = CalculateAmplifierRun(_input, phasePermutation.ToList(), stopAfterOneGo: true);
            //Console.WriteLine($"Phases {string.Join(",", phasePermutation)} give a result of {result}");
            max = Math.Max(max, result);
        }

        return max;
    }

    private Answer CalculatePart2Answer()
    {
        long max = 0;

        var phases = Enumerable.Range(5, 5).Select(x => (long)x).ToList();

        foreach (var phasePermutation in phases.Permutations())
        {
            var result = CalculateAmplifierRun(_input, phasePermutation.ToList(), stopAfterOneGo: false);
            //Console.WriteLine($"Phases {string.Join(",", phasePermutation)} give a result of {result}");
            max = Math.Max(max, result);
        }

        return max;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
