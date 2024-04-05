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

        // Whenever there is a hole followed by a ground (=true), jump to hit the ground (=false).
        var commands = new List<string>
        {
            "NOT C T", // T is notC
            "AND D T", // T is whether I should jump - if C is a hole and D is the ground
            "OR T J", // Put that in J
            "NOT B T", // T is notB
            "AND C T", // T is whether I should jump - if B is a hole and C is the ground
            "OR T J", // Put that in J
            "NOT A T", // T is notA
            "AND B T", // T is whether I should jump - if A is a hole and B is the ground
            "OR T J", // Put that in J
            "WALK",
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
                    Console.WriteLine($"Inputting '{commands[commandIndex]}'");
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
