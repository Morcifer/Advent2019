
namespace AdventOfCode;

public sealed class Day21 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day21() : this(RunMode.Real)
    {
    }

    public Day21(RunMode runMode)
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
        var computer = new Computer(_input);

        var commands = new List<string>() { "NOT A J", "WALK" };
        var commandIndex = 0;


        while (true)
        {
            var (returnMode, result) = computer.RunProgramToAsciiNewLine();

            switch (returnMode)
            {
                case ReturnMode.Terminate:
                    Console.WriteLine(result);
                    return -1;
                case ReturnMode.Output:
                    Console.WriteLine(result);
                    break;
                case ReturnMode.Input:
                    Console.WriteLine($"Inputting {commands[commandIndex]}");
                    computer.AddAsciiCommand(commands[commandIndex++]);
                    break;
            }
        }
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
