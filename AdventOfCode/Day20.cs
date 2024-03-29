namespace AdventOfCode;

using GridSpot = (int Row, int Column);

public sealed class Day20: BaseTestableDay
{
    private readonly List<string> _map;
    private readonly Dictionary<string, (GridSpot, GridSpot)> _portals;

    public Day20() : this(RunMode.Test)
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

        // The map should skip the first and last rows and columns.
        // And the portals are then found and marked.

        _map = input[2..^2]
            .Select(s => string.Join("", s[2..^2].Select(c => char.IsAsciiLetter(c) ? ' ' : c)))
            .ToList();

        var mapHeight = _map.Count;
        var mapLength = _map[0].Length;

        foreach (var row in _map)
        {
            Console.WriteLine(row);
        }

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
                portalLocations.Add((name, (row + 2, column)));
            }

            foreach (var row in new[] { topMidRow, input.Count - 2 })
            {
                if (!char.IsAsciiLetter(input[row][column]))
                {
                    continue;
                }

                var name = $"{input[row][column]}{input[row + 1][column]}";
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
                portalLocations.Add((name, (row, column + 2)));
            }

            foreach (var column in new[] { leftMidColumn, input[0].Length - 2 })
            {
                if (!char.IsAsciiLetter(input[row][column]))
                {
                    continue;
                }

                var name = $"{input[row][column]}{input[row][column + 1]}";
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
    }

    private (Dictionary<GridSpot, int> Distances, Dictionary<GridSpot, HashSet<char>> RequiredKeys) ShortestPath(GridSpot start)
    {
        var deltas = new List<GridSpot> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var queue = new Queue<(GridSpot Spot, HashSet<char> Keys, int Length)>();
        queue.Enqueue((start, new HashSet<char>(), 0));

        var explored = new HashSet<GridSpot>();

        var distances = new Dictionary<GridSpot, int>();
        var requiredKeys = new Dictionary<GridSpot, HashSet<char>>();

        while (queue.Count > 0)
        {
            var (spotToExplore, keysToExplore, lengthToExplore) = queue.Dequeue();

            if (explored.Contains(spotToExplore))
            {
                continue;
            }

            explored.Add(spotToExplore);

            if (spotToExplore.Row < 0 || spotToExplore.Row >= _map.Count)
            {
                continue;
            }

            if (spotToExplore.Column < 0 || spotToExplore.Column >= _map[spotToExplore.Row].Length)
            {
                continue;
            }

            var newKeys = keysToExplore.ToHashSet();
            var charToExplore = _map[spotToExplore.Row][spotToExplore.Column];

            if (_map[spotToExplore.Row][spotToExplore.Column] == '#')
            {
                continue;
            }

            if (char.IsAsciiLetterUpper(charToExplore)) // Door
            {
                var key = char.ToLower(charToExplore);
                newKeys.Add(key);
            }

            if (char.IsAsciiLetterLower(charToExplore)) // New key!
            {
                distances[spotToExplore] = lengthToExplore;
                requiredKeys[spotToExplore] = newKeys;
            }

            foreach (var neighborDelta in deltas)
            {
                var neighbour = (spotToExplore.Row + neighborDelta.Row, spotToExplore.Column + neighborDelta.Column);
                queue.Enqueue((neighbour, newKeys, lengthToExplore + 1));
            }
        }

        return (distances, requiredKeys);
    }

    private Answer CalculatePart1Answer()
    {
        foreach (var row in _map)
        {
            Console.WriteLine(row);
        }

        return -1;
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
