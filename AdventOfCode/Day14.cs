namespace AdventOfCode;

public sealed class Day14 : BaseTestableDay
{
    private readonly List<(List<(int Amount, string Material)> Inputs, (int Amount, string Material) Output)> _input;

    public Day14() : this(RunMode.Real)
    {
    }

    public Day14(RunMode runMode)
    {
        RunMode = runMode;

        _input = new List<(List<(int, string)>, (int, string))>();

        var lines = File.ReadAllLines(InputFilePath);

        foreach (var line in lines)
        {
            var leftAndRight = line.Split("=>").ToList();

            var leftList = new List<(int, string)>();

            foreach (var left in leftAndRight[0].Split(","))
            {
                var twoSplit = left.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
                leftList.Add((int.Parse(twoSplit[0]), twoSplit[1]));
            }

            var rightSplit = leftAndRight[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            _input.Add((leftList, (int.Parse(rightSplit[0]), rightSplit[1])));
        }
    }

    private Answer CalculatePart1Answer()
    {
        var conversions = _input
            .ToDictionary(
                x => x.Output.Material,
                x => (OutputAmount: x.Output.Amount, Inputs: x.Inputs)
            );

        var sources = _input
            .ToDictionary(
                x => x.Output.Material,
                x => _input.Count(y => y.Inputs.Any(t => t.Material == x.Output.Material))
            );

        var totalComponents = new Dictionary<string, int>() { { "FUEL", 1 } };

        while (totalComponents.Keys.Count > 1 || !totalComponents.ContainsKey("ORE"))
        {
            var firstNoneOre = totalComponents
                .Where(kvp => kvp.Key != "ORE")
                .OrderByDescending(kvp => totalComponents[kvp.Key] % conversions[kvp.Key].OutputAmount == 0) // round-number conversions first
                .ThenByDescending(kvp => conversions[kvp.Key].Inputs.Count)
                .ThenBy(kvp => sources[kvp.Key])
                .First()
                .Key;

            var (reactionOutputAmount, reactionInputs) = conversions[firstNoneOre];
            var multiple = (double)totalComponents[firstNoneOre] / reactionOutputAmount;

            //Console.WriteLine($"Before handling {firstNoneOre}'s (multiple {multiple} -> {(int)Math.Ceiling(multiple)}): {string.Join(", ", totalComponents.Select(kvp => $"{kvp.Value} {kvp.Key}"))}");

            totalComponents.Remove(firstNoneOre);

            foreach (var reactionInput in reactionInputs)
            {
                totalComponents.TryAdd(reactionInput.Material, 0);
                totalComponents[reactionInput.Material] += (int)Math.Ceiling(multiple) * reactionInput.Amount;
            }

            //Console.WriteLine($"After handling {firstNoneOre}'s (multiple {multiple}): {string.Join(", ", totalComponents.Select(kvp => $"{kvp.Value} {kvp.Key}"))}");
        }

        return totalComponents["ORE"]; // 1039136 too high. 1037001 is also bad. 1016165 is also too high. 
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
