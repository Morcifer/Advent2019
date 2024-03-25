namespace AdventOfCode;

using GridSpot = (int Row, int Column);

public sealed class Day18 : BaseTestableDay
{
    private readonly List<string> _map;
    private readonly Dictionary<char, GridSpot> _keys;

    public Day18() : this(RunMode.Real)
    {
    }

    public Day18(RunMode runMode)
    {
        RunMode = runMode;

        _map = File
            .ReadAllLines(InputFilePath)
            .ToList();

        _keys = _map
            .SelectMany((row, rowIndex) => row.ToCharArray().Select((character, columnIndex) => (rowIndex, columnIndex, character)))
            .Where(t => char.IsAsciiLetterLower(t.character))
            .ToDictionary(
                t => t.character,
                t => (t.rowIndex, t.columnIndex)
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

    private Answer RunKeySearch(List<GridSpot> entrances)
    {
        var importantSpots = _keys.Values.Concat(entrances).ToList();

        var distances = new Dictionary<(GridSpot, GridSpot), int>();
        var requiredKeys = new Dictionary<(GridSpot, GridSpot), HashSet<char>>();

        foreach (var importantSpot in importantSpots)
        {
            var (ds, ks) = ShortestPath(importantSpot);

            foreach (var key in ds.Keys)
            {
                distances[(importantSpot, key)] = ds[key];
                requiredKeys[(importantSpot, key)] = ks[key];
            }
        }

        var queue = new PriorityQueue<(GridSpot[] Bots, HashSet<char> Keys, int Length), int>();
        queue.Enqueue(
            (entrances.ToArray(), new HashSet<char>(), 0),
            0
        );

        var explored = new HashSet<(string Bots, string Keys)>();

        while (queue.Count > 0)
        {
            var (spotsToExplore, keysToExplore, lengthToExplore) = queue.Dequeue();

            if (keysToExplore.Count == _keys.Count)
            {
                return lengthToExplore;
            }

            var key = (
                string.Join(", ", spotsToExplore.Select(s => (s.Row, s.Column))),
                string.Join("", keysToExplore.OrderBy(c => c))
            );

            if (explored.Contains(key))
            {
                continue;
            }

            explored.Add(key);

            for (var botToMove = 0; botToMove < entrances.Count; botToMove++)
            {
                foreach (var (newTargetKey, newTargetSpot) in _keys)
                {
                    if (keysToExplore.Contains(newTargetKey))
                    {
                        continue;
                    }

                    var spotToExplore = spotsToExplore[botToMove];

                    // Can't reach this key for this bot.
                    if (!requiredKeys.ContainsKey((spotToExplore, newTargetSpot)))
                    {
                        continue;
                    }

                    var neededKeysToTarget = requiredKeys[(spotToExplore, newTargetSpot)];

                    // We don't have the keys for this yet
                    if (!neededKeysToTarget.IsSubsetOf(keysToExplore))
                    {
                        continue;
                    }

                    var neighbourBots = spotsToExplore.ToArray();
                    neighbourBots[botToMove] = newTargetSpot;

                    var newKeys = keysToExplore.ToHashSet();
                    newKeys.Add(newTargetKey);

                    var lengthToTarget = distances[(spotToExplore, newTargetSpot)];
                    var newLength = lengthToExplore + lengthToTarget;

                    queue.Enqueue((neighbourBots, newKeys, newLength), newLength);
                }
            }
        }

        return -1;
    }

    private Answer CalculatePart1Answer()
    {
        //foreach (var row in _map)
        //{
        //    Console.WriteLine(row);
        //}

        var entrance = Enumerable.Range(0, _map.Count)
            .SelectMany(row => Enumerable.Range(0, _map[row].Length).Select(column => (Row: row, Column: column)))
            .First(spot => _map[spot.Row][spot.Column] == '@');

        return RunKeySearch(new List<GridSpot>() { entrance });
    }

    private Answer CalculatePart2Answer()
    {
        var entrance = Enumerable.Range(0, _map.Count)
            .SelectMany(row => Enumerable.Range(0, _map[row].Length).Select(column => (Row: row, Column: column)))
            .First(spot => _map[spot.Row][spot.Column] == '@');

        // Replace part 1 entrance with 4 separate entrances by hacking it, because I'm too lazy to make a new one.
        _map[entrance.Row - 1] = _map[entrance.Row - 1][..(entrance.Column - 1)] + "@#@" + _map[entrance.Row - 1][(entrance.Column + 2)..];
        _map[entrance.Row] = _map[entrance.Row][..(entrance.Column - 1)] + "###" + _map[entrance.Row][(entrance.Column + 2)..];
        _map[entrance.Row + 1] = _map[entrance.Row + 1][..(entrance.Column - 1)] + "@#@" + _map[entrance.Row + 1][(entrance.Column + 2)..];

        //foreach (var row in _map)
        //{
        //    Console.WriteLine(row);
        //}

        var entrances = new List<GridSpot>()
        {
            (entrance.Row - 1, entrance.Column - 1),
            (entrance.Row - 1, entrance.Column + 1),
            (entrance.Row + 1, entrance.Column - 1),
            (entrance.Row + 1, entrance.Column + 1),
        };

        return RunKeySearch(entrances);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
