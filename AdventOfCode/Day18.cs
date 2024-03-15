using System.Linq;

namespace AdventOfCode;

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
        var deltas = new List<(int Row, int Column)> { (0, 1), (0, -1), (1, 0), (-1, 0) };

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

        var queue = new Queue<((int Row, int Column) Spot, HashSet<char> Keys, int Length)>();
        queue.Enqueue((entrance, new HashSet<char>(), 0));

        var explored = new HashSet<((int Row, int Column) Spot, string Keys)>();

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
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
