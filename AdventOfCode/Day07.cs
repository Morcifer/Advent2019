using MoreLinq.Extensions;

namespace AdventOfCode;

public sealed class Day07 : BaseTestableDay
{
    private readonly List<int> _input;

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
            .Select(int.Parse)
            .ToList();
    }

    internal static int CalculateAmplifierRun(List<int> program, List<int> thrusters, bool stopAfterOneGo)
    {
        var signal = 0;

        while (true)
        {
            for (var amplifier = 0; amplifier < 5; amplifier++)
            {
                var inputs = new List<int>() { thrusters[amplifier], signal };
                var outputs = Computer.RunProgram(program.ToList(), inputs);
                signal = outputs[0];
            }

            if (stopAfterOneGo)
            {
                break;
            }
        }

        return signal;
    }

    private Answer CalculatePart1Answer()
    {
        var max = 0;

        var phases = Enumerable.Range(0, 5).ToList();

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
        return -1;

        var max = 0;

        var phases = Enumerable.Range(5, 5).ToList();

        foreach (var phasePermutation in phases.Permutations())
        {
            var result = CalculateAmplifierRun(_input, phasePermutation.ToList(), stopAfterOneGo: true);
            Console.WriteLine($"Phases {string.Join(",", phasePermutation)} give a result of {result}");
            max = Math.Max(max, result);
        }

        return max;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
