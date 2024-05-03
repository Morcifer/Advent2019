using MoreLinq.Extensions;

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
            "take antenna",
            "east",
            "take ornament",
            "north",
            "west",
            "take fixed point",
            "east",
            "south",
            "west",
            "north",
            "west",
            "south",
            "take hologram",
            "north",
            "west",
            "take astronaut ice cream",
            "east",
            "east",
            "north",
            "take asterisk",
            "south",
            "south",
            "west",
            "north",
            // "take giant electromagnet", Stuck
            "south",
            "south",
            // "take photons", Eaten by Grue
            "south",
            "south",
            "take dark matter",
            "east",
            // "take infinite loop", You take the infinite loop. You take the infinite loop. You take the infinite loop...
            "west",
            "north",
            "west",
            // "take molten lava", You melt
            "north",
            "take monolith",
            "north",
            // "take escape pod", You're launched into space! Bye!
            "north",
            "east",
        };

        var commandIndex = 0;

        var items = new List<string>() { "fixed point", "monolith", "antenna", "astronaut ice cream", "hologram", "ornament", "dark matter", "asterisk" };

        foreach (var item in items)
        {
            commands.Add($"drop {item}");
        }

        var allSubsets = items.Subsets().ToList();
        var subsetIndex = 0;

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
                    if (commandIndex >= commands.Count)
                    {
                        foreach (var item in allSubsets[subsetIndex++])
                        {
                            commands.Add($"drop {item}");
                        }

                        foreach (var item in allSubsets[subsetIndex])
                        {
                            commands.Add($"take {item}");
                        }

                        commands.Add("inv");
                        commands.Add("east");
                    }

                    var command = commands[commandIndex++];
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
