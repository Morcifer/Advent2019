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

    private Dictionary<string, int> GetDistancesFromOre(Dictionary<string, (int OutputAmount, List<(int Amount, string Material)> Inputs)> conversions)
    {
        var distancesFromOre = new Dictionary<string, int>() { { "ORE", 0 } };
        while (distancesFromOre.Count <= conversions.Count)
        {
            foreach (var conversion in conversions)
            {
                if (distancesFromOre.ContainsKey(conversion.Key))
                {
                    continue;
                }

                if (!conversion.Value.Inputs.All(t => distancesFromOre.ContainsKey(t.Material)))
                {
                    continue;
                }

                distancesFromOre[conversion.Key] = conversion.Value.Inputs.Select(i => distancesFromOre[i.Material]).Max() + 1;
            }
        }

        return distancesFromOre;
    }

    private Answer CalculatePart1Answer()
    {
        var conversions = _input
            .ToDictionary(
                x => x.Output.Material,
                x => (OutputAmount: x.Output.Amount, Inputs: x.Inputs)
            );

        var distancesFromOre = GetDistancesFromOre(conversions);

        var neededMaterials = new Dictionary<string, int>() { { "FUEL", 1 } };

        while (neededMaterials.Count > 1 || !neededMaterials.ContainsKey("ORE"))
        {
            var firstNoneOre = neededMaterials
                .Where(kvp => kvp.Key != "ORE")
                .MaxBy(kvp => distancesFromOre[kvp.Key])
                .Key;

            var (reactionOutputAmount, reactionInputs) = conversions[firstNoneOre];
            var multiple = (int)Math.Ceiling((double)neededMaterials[firstNoneOre] / reactionOutputAmount);

            neededMaterials.Remove(firstNoneOre);

            foreach (var reactionInput in reactionInputs)
            {
                neededMaterials.TryAdd(reactionInput.Material, 0);
                neededMaterials[reactionInput.Material] += multiple * reactionInput.Amount;
            }
        }

        return neededMaterials["ORE"];
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
