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

    private Answer WalkOrRun(List<string> commands)
    {
        var computer = new Computer(_input);
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

    private Answer CalculatePart1Answer()
    {
        // Whenever there is a hole (= false) followed by a ground (= true), jump to hit the ground.
        var commands = new List<string>
        {
            "NOT C T", // T is !C, so it's true when it's a hole
            "AND D T", // T is whether I should jump - if C is a hole and D is ground
            "OR T J", // Put that in J
            "NOT B T", // T is !B, so it's true when it's a hole
            "AND C T", // T is whether I should jump - if B is a hole and C is ground
            "OR T J", // Put that in J
            "NOT A T", // T is !A, so it's true when it's a hole
            "AND B T", // T is whether I should jump - if A is a hole and B is ground
            "OR T J", // Put that in J

            "WALK",
        };

        return WalkOrRun(commands);
    }

    private Answer CalculatePart2Answer()
    {
        // Whenever there is a hole followed by a ground (=true), jump to hit the ground.
        // But make sure that where you land (D) is land, and it either lets you walk (E), or lets you jump (H).
        var commands = new List<string>
        {
            "NOT C T", // Same as before
            "AND D T",
            "OR T J", 
            "NOT B T",
            "AND C T",
            "OR T J", 
            "NOT A T",
            "AND B T",
            "OR T J",

            "OR J T", // Copy J into T: If J is true it goes into T. If it's false, it's because T was already false.

            "AND D T", // Why did I not need this to be in the previous one? Because the input was built for it?
            "AND D J",

            "AND E T", // E is ground so I can walk to it.
            "AND H J", // Or H should so I can jump there.
            "OR T J",

            "RUN",
        };

        return WalkOrRun(commands);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
