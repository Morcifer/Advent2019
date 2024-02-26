using System.Diagnostics;

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

    private Answer CalculatePart1Answer()
    {
        var computer = new Computer(_input, new List<long>() { 1 });
        var output = computer.RunProgramToTermination();

        Debug.Assert(output.Count == 1);

        return output[0];  // 203 is too low
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
