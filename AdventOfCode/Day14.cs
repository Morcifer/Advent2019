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

    private long GetRequiredOre(long requiredFuel)
    {
        var conversions = _input
            .ToDictionary(
                x => x.Output.Material,
                x => (OutputAmount: x.Output.Amount, Inputs: x.Inputs)
            );

        var distancesFromOre = GetDistancesFromOre(conversions);

        var neededMaterials = new Dictionary<string, long>() { { "FUEL", requiredFuel } };

        while (neededMaterials.Count > 1 || !neededMaterials.ContainsKey("ORE"))
        {
            var firstNoneOre = neededMaterials
                .Where(kvp => kvp.Key != "ORE")
                .MaxBy(kvp => distancesFromOre[kvp.Key])
                .Key;

            var (reactionOutputAmount, reactionInputs) = conversions[firstNoneOre];
            var multiple = (long)Math.Ceiling((double)neededMaterials[firstNoneOre] / reactionOutputAmount);

            neededMaterials.Remove(firstNoneOre);

            foreach (var reactionInput in reactionInputs)
            {
                neededMaterials.TryAdd(reactionInput.Material, 0);
                neededMaterials[reactionInput.Material] += multiple * reactionInput.Amount;
            }
        }

        return neededMaterials["ORE"];
    }

    private Answer CalculatePart1Answer()
    {
        return GetRequiredOre(1);
    }

    private Answer CalculatePart2Answer()
    {
        var availableOre = 1000000000000;
        var singleFuelOreAmount = GetRequiredOre(1);

        var targetFuel = availableOre / singleFuelOreAmount; // Estimate fuel generation.
        var oreForTargetFuel = GetRequiredOre(targetFuel);

        while (true) // Start increasing the fuel amount until you overshot
        {
            var possibleExtraFuel = (long)Math.Floor((double)(availableOre - oreForTargetFuel) / singleFuelOreAmount);
            possibleExtraFuel = Math.Max(possibleExtraFuel, 1);

            targetFuel += possibleExtraFuel;
            oreForTargetFuel = GetRequiredOre(targetFuel);

            //Console.WriteLine($"Required ore for {targetFuel} fuel is {oreForTargetFuel} (vs. {availableOre})");

            if (oreForTargetFuel >= availableOre)
            {
                var correction = oreForTargetFuel == availableOre ? 0 : -1;
                return targetFuel + correction;
            }
        }
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
