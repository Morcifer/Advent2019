namespace AdventOfCode;

public sealed class Day06 : BaseTestableDay
{
    private readonly Dictionary<string, string> _input;

    public Day06() : this(RunMode.Real)
    {
    }

    public Day06(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(r => r.Split(")").ToList())
            .ToDictionary(x => x[1], x => x[0]);
    }

    private List<string> GetOrbits(string spaceObject)
    {
        var result = new List<string>();

        var current = spaceObject;

        while (_input.ContainsKey(current))
        {
            result.Add(current);
            current = _input[current];
        }

        return result;
    }

    private Answer CalculatePart1Answer()
    {
        var result = 0;

        // There's a more efficient way to calculate this, with dynamic programming or memoization,
        // but there doesn't appear to be a need.
        foreach (var spaceObject in _input.Keys)
        {
            var orbits = GetOrbits(spaceObject);

            //Console.WriteLine($"Object {spaceObject} has {orbits.Count} direct and indirect orbits");
            result += orbits.Count;
        }

        return result;
    }

    private Answer CalculatePart2Answer()
    {
        var myOrbits = GetOrbits("YOU");
        var santaOrbits = GetOrbits("SAN");

        foreach (var spaceObject in myOrbits)
        {
            // This can possibly also be done more efficiently (even if we take overhead into account), but what's the point?
            if (santaOrbits.Contains(spaceObject))
            {
                return myOrbits.IndexOf(spaceObject) + santaOrbits.IndexOf(spaceObject) - 2;
            }
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
