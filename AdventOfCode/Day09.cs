using System.Runtime.CompilerServices;

namespace AdventOfCode;

public sealed class Day09 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day09() : this(RunMode.Real)
    {
    }

    public Day09(RunMode runMode)
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

                var output = computers[amplifier].RunProgram();

                if (!output.HasValue) // Termination...
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
        var computer = new Computer(_input, new List<long>() { 1 });
        var output = computer.RunProgramToTermination();

        return output[0];  // 203 is too low
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
