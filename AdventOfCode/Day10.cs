using MathNet.Numerics;

namespace AdventOfCode;

public sealed class Day10 : BaseTestableDay
{
    // TODO 1: use a sparse representation, even though it looks like it's unnecessary.
    // TODO 2: Introduce grid coordinate struct that has (x, y) and (row, column), because this is silly!
    private readonly List<string> _input;

    public Day10() : this(RunMode.Real)
    {
    }

    public Day10(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private Dictionary<(int DY, int DX), List<(int DY, int DX)>> AsteroidsPerLineOfSight(int monitoringY, int monitoringX)
    {
        var result = new Dictionary<(int DY, int DX), List<(int DY, int DX)>>();

        for (var y = 0; y < _input.Count; y++)
        {
            for (var x = 0; x < _input[0].Length; x++)
            {
                if (_input[y][x] != '#')
                {
                    continue;
                }

                var deltaY = y - monitoringY;
                var deltaX = x - monitoringX;

                var greatestCommonDivisor = Euclid.GreatestCommonDivisor(deltaY, deltaX);

                var key = greatestCommonDivisor == 0
                    ? (deltaY, deltaX)
                    : ((int)(deltaY / greatestCommonDivisor), (int)(deltaX / greatestCommonDivisor));

                if (!result.ContainsKey(key))
                {
                    result[key] = new List<(int, int)>();
                }

                result[key].Add((deltaY, deltaX));
            }
        }

        return result;
    }

    private ((int Y, int X) Location, Dictionary<(int DY, int DX), List<(int DY, int DX)>> Dictionary) BestStationLocation()
    {
        var allResults = new List<(int Visible, (int Y, int X) Location, Dictionary<(int DY, int DX), List<(int DY, int DX)>> Result)>();

        for (var y = 0; y < _input.Count; y++)
        {
            for (var x = 0; x < _input[0].Length; x++)
            {
                if (_input[y][x] != '#')
                {
                    continue;
                }

                var result = AsteroidsPerLineOfSight(y, x);
                allResults.Add((result.Keys.Count - 1, (y, x), result));
            }
        }

        var best = allResults.MaxBy(x => x.Visible);

        return (best.Location, best.Result);
    }

    private Answer CalculatePart1Answer()
    {
        var best = BestStationLocation();
        return best.Dictionary.Count - 1;
    }

    private Answer CalculatePart2Answer()
    {
        var (bestLocation, bestDictionary) = BestStationLocation();
        bestDictionary.Remove((0, 0)); // Don't cut the branch you're sitting on.

        var byDirection = bestDictionary
            .OrderByDescending(kvp => kvp.Key.DX >= 0) // Right side, then left side
            .ThenBy(kvp => Math.Atan2((double)kvp.Key.DX, -kvp.Key.DY))
            .Select(kvp => kvp.Value
                .OrderBy(x => Math.Abs(x.DX) + Math.Abs(x.DY))
                .Select(x => (X: x.DX + bestLocation.X, Y: x.DY + bestLocation.Y))
                .ToList()
            )
            .ToList();

        var spaceLaseredDirectionIndex = 0;

        for (var laseredCounter = 0; laseredCounter < 200; laseredCounter++)
        {
            var spaceLasered = byDirection[spaceLaseredDirectionIndex].First();

            //Console.WriteLine($"{laseredCounter + 1}: ({spaceLasered.X}, {spaceLasered.Y}) has been space-lasered!!");

            byDirection[spaceLaseredDirectionIndex].RemoveAt(0);

            if (!byDirection[spaceLaseredDirectionIndex].Any())
            {
                byDirection.RemoveAt(spaceLaseredDirectionIndex);
                spaceLaseredDirectionIndex--;
            }

            if (laseredCounter == 199)
            {
                return spaceLasered.X * 100 + spaceLasered.Y;
            }

            spaceLaseredDirectionIndex = (spaceLaseredDirectionIndex + 1) % byDirection.Count;
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
