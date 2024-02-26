namespace AdventOfCode;

public sealed class Day05 : BaseTestableDay
{
    private readonly List<int> _input;

    public Day05() : this(RunMode.Real)
    {
    }

    public Day05(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(int.Parse)
            .ToList();
    }

    private Answer GetDiagnosticsCode()
    {
        var program = _input.ToList();
        var computer = new Computer(program, new List<int>() { 1 });
        return computer.RunProgramToTermination()[^1];
    }

    private Answer FindNounAndVerb()
    {
        var program = _input.ToList();
        var computer = new Computer(program, new List<int>() { 5 });
        return computer.RunProgramToTermination()[^1];
    }

    public override ValueTask<string> Solve_1() => new(GetDiagnosticsCode());

    public override ValueTask<string> Solve_2() => new(FindNounAndVerb());
}
