namespace AdventOfCode;

using GridSpot = (int Row, int Column);

public sealed class Day20: BaseTestableDay
{
    private readonly List<string> _map;
    private readonly Dictionary<string, (GridSpot, GridSpot)> _portals;
    private readonly Dictionary<GridSpot, GridSpot> _portalsFromTo;

    public Day20() : this(RunMode.Real)
    {
    }

    public Day20(RunMode runMode)
    {
        RunMode = runMode;

        var input = File
            .ReadAllLines(InputFilePath)
            .ToList();

        var inputHeight = input.Count;
        var inputLength = input[0].Length;

        _map = input[2..^2]
            .Select(s => string.Join("", s[2..^2].Select(c => char.IsAsciiLetter(c) ? ' ' : c)))
            .ToList();

        var mapHeight = _map.Count;
        var mapLength = _map[0].Length;

        //foreach (var row in _map)
        //{
        //    Console.WriteLine(row);
        //}

        var portalLocations = new List<(string, GridSpot)>(); // These are grid spots in the input, not the map.

        // Vertical portals
        var topMidRow = Enumerable.Range(0, mapHeight).First(row => _map[row].Contains(' ')) + 2;
        var bottomMidRow = Enumerable.Range(0, mapHeight).Last(row => _map[row].Contains(' ')) + 2 - 1; // offset from map, but then take row above

        for (var column = 2; column < inputLength - 1; column++)
        {
            foreach (var row in new[] { 0, bottomMidRow })
            {
                if (!char.IsAsciiLetter(input[row][column]))
                {
                    continue;
                }

                var name = $"{input[row][column]}{input[row + 1][column]}";

                if (name.Contains(' '))
                {
                    continue;
                }

                portalLocations.Add((name, (row + 2, column)));
            }

            foreach (var row in new[] { topMidRow, input.Count - 2 })
            {
                if (!char.IsAsciiLetter(input[row][column]))
                {
                    continue;
                }

                var name = $"{input[row][column]}{input[row + 1][column]}";

                if (name.Contains(' '))
                {
                    continue;
                }

                portalLocations.Add((name, (row - 1, column)));
            }
        }

        // Horizontal portals
        var leftMidColumn = Enumerable.Range(0, mapLength).First(column => _map.Any(row => row[column] == ' ')) + 2;
        var rightMidColumn = Enumerable.Range(0, mapLength).Last(column => _map.Any(row => row[column] == ' ')) + 2 - 1; // offset from map, but then take row above

        for (var row = 2; row < inputHeight - 1; row++)
        {
            foreach (var column in new[] { 0, rightMidColumn })
            {
                if (!char.IsAsciiLetter(input[row][column]))
                {
                    continue;
                }

                var name = $"{input[row][column]}{input[row][column + 1]}";

                if (name.Contains(' '))
                {
                    continue;
                }

                portalLocations.Add((name, (row, column + 2)));
            }

            foreach (var column in new[] { leftMidColumn, input[0].Length - 2 })
            {
                if (!char.IsAsciiLetter(input[row][column]))
                {
                    continue;
                }

                var name = $"{input[row][column]}{input[row][column + 1]}";

                if (name.Contains(' '))
                {
                    continue;
                }

                portalLocations.Add((name, (row, column - 1)));
            }
        }

        _portals = portalLocations
            .Select(t => (t.Item1, (t.Item2.Row - 2, t.Item2.Column - 2)))
            .GroupBy(t => t.Item1)
            .ToDictionary(
                g => g.Key,
                g => g.Count() == 1 ? (g.First().Item2, g.First().Item2) : (g.First().Item2, g.Skip(1).First().Item2)
            );

        _portalsFromTo = _portals
            .Where(kvp => kvp.Key != "AA" && kvp.Key != "ZZ")
            .Select(kvp => kvp.Value)
            .SelectMany(t => new List<(GridSpot, GridSpot)> { (t.Item1, t.Item2), (t.Item2, t.Item1) })
            .ToDictionary(t => t.Item1, t => t.Item2);
    }

    private int ShortestPath(GridSpot start, GridSpot end, bool ignoreLevels)
    {
        var deltas = new List<GridSpot> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var queue = new Queue<(GridSpot Spot, int Level, int Length)>();
        queue.Enqueue((start, 0, 0));

        var explored = new HashSet<(GridSpot, int)>();

        while (queue.Count > 0)
        {
            var (spotToExplore, levelToExplore, lengthToExplore) = queue.Dequeue();

            if (explored.Contains((spotToExplore, levelToExplore)))
            {
                continue;
            }

            explored.Add((spotToExplore, levelToExplore));

            if (spotToExplore == end && (ignoreLevels || levelToExplore == 0))
            {
                return lengthToExplore;
            }

            if (_portalsFromTo.TryGetValue(spotToExplore, out var otherSide))
            {
                var outer = spotToExplore.Row == 0
                            || spotToExplore.Column == 0
                            || spotToExplore.Row == _map.Count - 1
                            || spotToExplore.Column == _map[spotToExplore.Row].Length - 1;

                var newLevel = outer ? levelToExplore - 1 : levelToExplore + 1;

                if (ignoreLevels || newLevel >= 0)
                {
                    queue.Enqueue((otherSide, newLevel, lengthToExplore + 1));
                }
            }

            foreach (var neighborDelta in deltas)
            {
                var neighbour = new GridSpot(spotToExplore.Row + neighborDelta.Row, spotToExplore.Column + neighborDelta.Column);

                if (neighbour.Row < 0 || neighbour.Row >= _map.Count)
                {
                    continue;
                }

                if (neighbour.Column < 0 || neighbour.Column >= _map[neighbour.Row].Length)
                {
                    continue;
                }

                if (_map[neighbour.Row][neighbour.Column] is '#' or ' ')
                {
                    continue;
                }

                queue.Enqueue((neighbour, levelToExplore, lengthToExplore + 1));
            }
        }

        return -1;
    }

    private Answer CalculatePart1Answer()
    {
        return ShortestPath(_portals["AA"].Item1, _portals["ZZ"].Item1, true);
    }

    private Answer CalculatePart2Answer()
    {
        return ShortestPath(_portals["AA"].Item1, _portals["ZZ"].Item1, false);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
