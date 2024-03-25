namespace AdventOfCode;

using System.Linq;
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

    private (Dictionary<(GridSpot, GridSpot), int> Distances, Dictionary<(GridSpot, GridSpot), HashSet<char>> RequiredKeys) FloydWarshall(List<string> map)
    {
        var vertices = Enumerable.Range(0, map.Count)
            .SelectMany(row => Enumerable.Range(0, map[row].Length).Select(column => (Row: row, Column: column)))
            .Where(spot => map[spot.Row][spot.Column] != '#')
            .ToList();

        var distances = vertices
            .SelectMany(from => vertices.Select(to => (from, to)))
            .ToDictionary(t => (t.from, t.to), _ => 1000000); // I miss infinity.

        var requiredKeys = distances
            .Keys
            .ToDictionary(t => t, _ => new HashSet<char>());

        var deltas = new List<GridSpot> { (0, 0), (0, 1), (0, -1), (1, 0), (-1, 0)};

        foreach (var vertex in vertices)
        {
            foreach (var delta in deltas)
            {
                GridSpot neighbour = (vertex.Row + delta.Row, vertex.Column + delta.Column);

                if (neighbour.Row < 0 || neighbour.Row >= map.Count)
                {
                    continue;
                }

                if (neighbour.Column < 0 || neighbour.Column >= map[neighbour.Row].Length)
                {
                    continue;
                }

                if (map[neighbour.Row][neighbour.Column] == '#')
                {
                    continue;
                }

                // I prefer repeat assignments to extra code in this case.
                distances[(vertex, neighbour)] = vertex == neighbour ? 0 : 1;
                distances[(neighbour, vertex)] = vertex == neighbour ? 0 : 1;

                if (char.IsAsciiLetterUpper(map[vertex.Row][vertex.Column]))
                {
                    var key = char.ToLower(map[vertex.Row][vertex.Column]);
                    requiredKeys[(vertex, neighbour)].Add(key);
                    requiredKeys[(neighbour, vertex)].Add(key);
                }

                if (char.IsAsciiLetterUpper(map[neighbour.Row][neighbour.Column]))
                {
                    var key = char.ToLower(map[neighbour.Row][neighbour.Column]);
                    requiredKeys[(vertex, neighbour)].Add(key);
                    requiredKeys[(neighbour, vertex)].Add(key);
                }
            }
        }

        foreach (var kVertex in vertices)
        {
            foreach (var iVertex in vertices)
            {
                foreach (var jVertex in vertices)
                {
                    if (distances[(iVertex, jVertex)] <= distances[(iVertex, kVertex)] + distances[(kVertex, jVertex)])
                    {
                        continue;
                    }

                    distances[(iVertex, jVertex)] = distances[(iVertex, kVertex)] + distances[(kVertex, jVertex)];

                    var newKeys = requiredKeys[(iVertex, kVertex)].ToHashSet();
                    newKeys.UnionWith(requiredKeys[(kVertex, jVertex)]);

                    requiredKeys[(iVertex, jVertex)] = newKeys;
                }
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

        Console.WriteLine("Calculating Floyd Warshall");
        var (distances, requiredKeys) = FloydWarshall(_map);
        Console.WriteLine("Floyd Warshall calculated");


        var entrance = Enumerable.Range(0, _map.Count)
            .SelectMany(row => Enumerable.Range(0, _map[row].Length).Select(column => (Row: row, Column: column)))
            .First(spot => _map[spot.Row][spot.Column] == '@');

        var queue = new PriorityQueue<(GridSpot Spot, HashSet<char> Keys, int Length), int>();
        queue.Enqueue((entrance, new HashSet<char>(), 0), 0);

        var explored = new HashSet<(GridSpot Spot, string Keys)>();

        while (queue.Count > 0)
        {
            var (spotToExplore, keysToExplore, lengthToExplore) = queue.Dequeue();

            if (queue.Count % 1000 == 0)
            {
                Console.WriteLine($"Queue size is {queue.Count}, keys: {string.Join(", ", keysToExplore.OrderBy(c => c))} ({keysToExplore.Count} out of {_keys.Count})");
            }

            if (keysToExplore.Count == _keys.Count)
            {
                return lengthToExplore;
            }

            // Do I know this tree?
            if (explored.Contains((spotToExplore, string.Join("", keysToExplore.OrderBy(c => c)))))
            {
                continue;
            }

            explored.Add((spotToExplore, string.Join("", keysToExplore.OrderBy(c => c))));

            foreach (var (newTargetKey, newTargetSpot) in _keys)
            {
                // We don't need this key anymore.
                if (keysToExplore.Contains(newTargetKey))
                {
                    continue;
                }

                var neededKeysToTarget = requiredKeys[(spotToExplore, newTargetSpot)];
                
                // We don't have the keys for this yet
                if (!neededKeysToTarget.IsSubsetOf(keysToExplore))
                {
                    continue;
                }

                var newKeys = keysToExplore.ToHashSet();
                newKeys.Add(newTargetKey);

                var lengthToTarget = distances[(spotToExplore, newTargetSpot)];
                var newLength = lengthToExplore + lengthToTarget;

                queue.Enqueue((newTargetSpot, newKeys, newLength), newLength);
            }
        }

        return -1;
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
        var entrance = Enumerable.Range(0, _map.Count)
            .SelectMany(row => Enumerable.Range(0, _map[row].Length).Select(column => (Row: row, Column: column)))
            .First(spot => _map[spot.Row][spot.Column] == '@');

        // Replace part 1 entrance with 4 separate entrances
        _map[entrance.Row - 1] = _map[entrance.Row - 1][..(entrance.Column - 1)] + "@#@" + _map[entrance.Row - 1][(entrance.Column + 2)..];
        _map[entrance.Row] = _map[entrance.Row][..(entrance.Column - 1)] + "###" + _map[entrance.Row][(entrance.Column + 2)..];
        _map[entrance.Row + 1] = _map[entrance.Row + 1][..(entrance.Column - 1)] + "@#@" + _map[entrance.Row + 1][(entrance.Column + 2)..];

        foreach (var row in _map)
        {
            Console.WriteLine(row);
        }

        Console.WriteLine("Calculating Floyd Warshall");
        var (distances, requiredKeys) = FloydWarshall(_map);
        Console.WriteLine("Floyd Warshall calculated");

        var entrances = new List<GridSpot>()
        {
            (entrance.Row - 1, entrance.Column - 1),
            (entrance.Row - 1, entrance.Column + 1),
            (entrance.Row + 1, entrance.Column - 1),
            (entrance.Row + 1, entrance.Column + 1),
        };

        var queue = new PriorityQueue<(GridSpot Spot1, GridSpot Spot2, GridSpot Spot3, GridSpot Spot4, HashSet<char> Keys, int Length), int>();
        queue.Enqueue((entrances[0], entrances[1], entrances[2], entrances[3], new HashSet<char>(), 0), 0);

        var explored = new HashSet<(GridSpot Spot1, GridSpot Spot2, GridSpot Spot3, GridSpot Spot4, string Keys)>();

        while (queue.Count > 0)
        {
            var (spotToExplore1, spotToExplore2, spotToExplore3, spotToExplore4, keysToExplore, lengthToExplore) = queue.Dequeue();

            if (queue.Count % 1000 == 0)
            {
                Console.WriteLine($"Queue size is {queue.Count}, keys: {string.Join(", ", keysToExplore.OrderBy(c => c))} ({keysToExplore.Count} out of {_keys.Count})");
            }

            if (keysToExplore.Count == _keys.Count)
            {
                return lengthToExplore;
            }

            var key = (spotToExplore1, spotToExplore2, spotToExplore3, spotToExplore4, string.Join("", keysToExplore.OrderBy(c => c)));

            if (explored.Contains(key))
            {
                continue;
            }

            explored.Add(key);

            for (var botToMove = 1; botToMove <= 4; botToMove++)
            {
                foreach (var (newTargetKey, newTargetSpot) in _keys)
                {
                    if (keysToExplore.Contains(newTargetKey))
                    {
                        continue;
                    }

                    var spotToExplore = botToMove switch
                    {
                        1 => spotToExplore1,
                        2 => spotToExplore2,
                        3 => spotToExplore3,
                        4 => spotToExplore4,
                    };

                    var neededKeysToTarget = requiredKeys[(spotToExplore, newTargetSpot)];

                    // We don't have the keys for this yet
                    if (!neededKeysToTarget.IsSubsetOf(keysToExplore))
                    {
                        continue;
                    }

                    var (neighbour1, neighbour2, neighbour3, neighbour4) = (spotToExplore1, spotToExplore2, spotToExplore3, spotToExplore4);
                    
                    switch (botToMove)
                    {
                        case 1:
                            neighbour1 = newTargetSpot;
                            break;
                        case 2:
                            neighbour2 = newTargetSpot;
                            break;
                        case 3:
                            neighbour3 = newTargetSpot;
                            break;
                        case 4:
                            neighbour4 = newTargetSpot;
                            break;
                    }

                    var newKeys = keysToExplore.ToHashSet();
                    newKeys.Add(newTargetKey);

                    var lengthToTarget = distances[(spotToExplore, newTargetSpot)];
                    var newLength = lengthToExplore + lengthToTarget;

                    queue.Enqueue((neighbour1, neighbour2, neighbour3, neighbour4, newKeys, newLength), newLength);
                }
            }
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
