namespace AdventOfCode;

using GridSpot = (int Row, int Column);

public sealed class Day18 : BaseTestableDay
{
    private readonly List<string> _input;

    public Day18() : this(RunMode.Real)
    {
    }

    public Day18(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private Answer CalculatePart1Answer()
    {
        var deltas = new List<GridSpot> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        //foreach (var row in _input)
        //{
        //    Console.WriteLine(row);
        //}

        // Option 1:
        // Find locations of keys and doors.
        // Find shortest paths between all keys and all doors.
        // Might not be useful, if there are two possible ways and one passes through another key or can't pass through a door.

        // Option 2:
        // BFS with the nodes being location and keys in inventory (and length). Cut branch if you each a door you don't have the key for.
        var entrance = Enumerable.Range(0, _input.Count)
            .SelectMany(row => Enumerable.Range(0, _input[row].Length).Select(column => (Row: row, Column: column)))
            .First(spot => _input[spot.Row][spot.Column] == '@');

        var allKeys = _input
            .SelectMany(row => row.ToCharArray().Where(c => char.IsAsciiLetterLower(c)))
            .ToHashSet();

        var queue = new Queue<(GridSpot Spot, HashSet<char> Keys, int Length)>();
        queue.Enqueue((entrance, new HashSet<char>(), 0));

        var explored = new HashSet<(GridSpot Spot, string Keys)>();

        while (queue.Count > 0)
        {
            var (spotToExplore, keysToExplore, lengthToExplore) = queue.Dequeue();

            if (keysToExplore.Count == allKeys.Count)
            {
                return lengthToExplore - 1;
            }

            if (explored.Contains((spotToExplore, string.Join("", keysToExplore.OrderBy(c => c)))))
            {
                continue;
            }

            if (spotToExplore.Row < 0 || spotToExplore.Row >= _input.Count)
            {
                continue;
            }

            if (spotToExplore.Column < 0 || spotToExplore.Column >= _input[spotToExplore.Row].Length)
            {
                continue;
            }

            var newKeys = keysToExplore.ToHashSet();
            var charToExplore = _input[spotToExplore.Row][spotToExplore.Column];

            explored.Add((spotToExplore, string.Join("", newKeys.OrderBy(c => c))));

            if (charToExplore == '#')
            {
                continue;
            }

            if (char.IsAsciiLetterUpper(charToExplore) && !newKeys.Contains(char.ToLower(charToExplore))) // Door!
            {
                //Console.WriteLine($"Reached a dead end of door {charToExplore} at {spotToExplore}");
                continue;
            }

            if (char.IsAsciiLetterLower(charToExplore)) // New key!
            {
                //Console.WriteLine($"Found a key {charToExplore} at {spotToExplore} for a total of {string.Join(", ", newKeys.OrderBy(c => c))}");
                newKeys.Add(charToExplore);
            }

            foreach (var neighborDelta in deltas)
            {
                var neighbour = (spotToExplore.Row + neighborDelta.Row, spotToExplore.Column + neighborDelta.Column);
                queue.Enqueue((neighbour, newKeys, lengthToExplore + 1));
            }
        }

        return -1;
    }

    private Answer CalculatePart2Answer()
    {
        var deltas = new List<GridSpot> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var entrance = Enumerable.Range(0, _input.Count)
            .SelectMany(row => Enumerable.Range(0, _input[row].Length).Select(column => (Row: row, Column: column)))
            .First(spot => _input[spot.Row][spot.Column] == '@');

        // Replace part 1 entrance with 4 separate entrances
        _input[entrance.Row - 1] = _input[entrance.Row - 1][..(entrance.Column - 1)] + "@#@" + _input[entrance.Row - 1][(entrance.Column + 2)..];
        _input[entrance.Row] = _input[entrance.Row][..(entrance.Column - 1)] + "###" + _input[entrance.Row][(entrance.Column + 2)..];
        _input[entrance.Row + 1] = _input[entrance.Row + 1][..(entrance.Column - 1)] + "@#@" + _input[entrance.Row + 1][(entrance.Column + 2)..];

        foreach (var row in _input)
        {
            Console.WriteLine(row);
        }

        var entrances = new List<GridSpot>()
        {
            (entrance.Row - 1, entrance.Column - 1),
            (entrance.Row - 1, entrance.Column + 1),
            (entrance.Row + 1, entrance.Column - 1),
            (entrance.Row + 1, entrance.Column + 1),
        };

        var allKeys = _input
            .SelectMany(row => row.ToCharArray().Where(c => char.IsAsciiLetterLower(c)))
            .ToHashSet();

        var queue = new Queue<(GridSpot Spot1, GridSpot Spot2, GridSpot Spot3, GridSpot Spot4, int LatestMove, HashSet<char> Keys, int Length)>();
        queue.Enqueue((entrances[0], entrances[1], entrances[2], entrances[3], 1, new HashSet<char>(), 0));
        queue.Enqueue((entrances[0], entrances[1], entrances[2], entrances[3], 2, new HashSet<char>(), 0));
        queue.Enqueue((entrances[0], entrances[1], entrances[2], entrances[3], 3, new HashSet<char>(), 0));
        queue.Enqueue((entrances[0], entrances[1], entrances[2], entrances[3], 4, new HashSet<char>(), 0));

        var explored = new HashSet<(GridSpot Spot1, GridSpot Spot2, GridSpot Spot3, GridSpot Spot4, string Keys)>();

        while (queue.Count > 0)
        {
            var (spotToExplore1, spotToExplore2, spotToExplore3, spotToExplore4, latestMove, keysToExplore, lengthToExplore) = queue.Dequeue();

            if (queue.Count % 1000000 == 0)
            {
                Console.WriteLine($"Queue size is {queue.Count}, keys: {string.Join(", ", keysToExplore.OrderBy(c => c))} ({keysToExplore.Count} out of {allKeys.Count})");
            }

            if (keysToExplore.Count == allKeys.Count)
            {
                return lengthToExplore - 1;
            }

            var key = (spotToExplore1, spotToExplore2, spotToExplore3, spotToExplore4, string.Join("", keysToExplore.OrderBy(c => c)));

            if (explored.Contains(key))
            {
                continue;
            }

            explored.Add(key);

            var (rowToCheck, columnToCheck) = latestMove switch
            {
                1 => spotToExplore1,
                2 => spotToExplore2,
                3 => spotToExplore3,
                4 => spotToExplore4,
            };

            if (rowToCheck < 0 || rowToCheck >= _input.Count)
            {
                continue;
            }

            if (columnToCheck < 0 || columnToCheck >= _input[rowToCheck].Length)
            {
                continue;
            }

            var newKeys = keysToExplore.ToHashSet();
            var charToExplore = _input[rowToCheck][columnToCheck];

            if (charToExplore == '#')
            {
                continue;
            }

            if (char.IsAsciiLetterUpper(charToExplore) && !newKeys.Contains(char.ToLower(charToExplore))) // Door!
            {
                //Console.WriteLine($"Reached a dead end of door {charToExplore} at ({rowToCheck}, {columnToCheck})");
                continue;
            }

            if (char.IsAsciiLetterLower(charToExplore)) // New key!
            {
                //Console.WriteLine($"Found a key {charToExplore} at ({rowToCheck}, {columnToCheck}) for a total of {string.Join(", ", newKeys.OrderBy(c => c))}");
                newKeys.Add(charToExplore);
            }

            for (var botToMove = 1; botToMove <= 4; botToMove++)
            {
                foreach (var neighborDelta in deltas)
                {
                    var (neighbour1, neighbour2, neighbour3, neighbour4) = (spotToExplore1, spotToExplore2, spotToExplore3, spotToExplore4);
                    
                    switch (botToMove)
                    {
                        case 1:
                            neighbour1 = (neighbour1.Row + neighborDelta.Row, neighbour1.Column + neighborDelta.Column);
                            break;
                        case 2:
                            neighbour2 = (neighbour2.Row + neighborDelta.Row, neighbour2.Column + neighborDelta.Column);
                            break;
                        case 3:
                            neighbour3 = (neighbour3.Row + neighborDelta.Row, neighbour3.Column + neighborDelta.Column);
                            break;
                        case 4:
                            neighbour4 = (neighbour4.Row + neighborDelta.Row, neighbour4.Column + neighborDelta.Column);
                            break;
                    }

                    queue.Enqueue((neighbour1, neighbour2, neighbour3, neighbour4, botToMove, newKeys, lengthToExplore + 1));
                }
            }
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
