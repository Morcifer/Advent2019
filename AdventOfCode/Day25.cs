namespace AdventOfCode;

public sealed class Day25 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day25() : this(RunMode.Real)
    {
    }

    public Day25(RunMode runMode)
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
        var commands = new List<string>()
        {
            "east",
            //"take giant electromagnet",
            "south"
        };

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
                    if (long.TryParse(result, out var finalResult))
                    {
                        return finalResult;
                    }

                    Console.WriteLine(result);
                    break;
                case ReturnMode.Input:
                    var command = commandIndex >= commands.Count ? "inv" : commands[commandIndex++];

                    Console.WriteLine($"Inputting '{command}'");
                    computer.AddAsciiCommand(command);
                    break;
            }
        }
    }

    private Answer CalculatePart2Answer()
    {
        return - 1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
