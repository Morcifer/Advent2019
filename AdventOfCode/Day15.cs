using System.Diagnostics;
using MathNet.Numerics.RootFinding;

namespace AdventOfCode;

public enum MovementCommand
{
    North = 1,
    South = 2,
    West = 3,
    East = 4,
}

public enum StatusCode
{
    HitWall = 0,
    Moved = 1,
    HitOxygen = 2,
}

public enum TileType
{
    Unknown = 0,
    Empty = 1,
    Wall = 2,
    Oxygen = 3,
    Droid = 4,
    Origin = 5,
}

public sealed class Day15 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day15() : this(RunMode.Real)
    {
    }

    public Day15(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    private string PrintTile(TileType tileType)
    {
        return tileType switch
        {
            TileType.Unknown => " ",
            TileType.Empty => ".",
            TileType.Wall => "#",
            TileType.Oxygen => "X",
            TileType.Droid => "D",
            TileType.Origin => "O",
        };
    }

    private void PrintMap(Dictionary<(int Row, int Column), TileType> panels, (int Row, int Column) droid)
    {
        var minRow = panels.Keys.Min(x => x.Row);
        var maxRow = panels.Keys.Max(x => x.Row);
        var minColumn = panels.Keys.Min(x => x.Column);
        var maxColumn = panels.Keys.Max(x => x.Column);

        for (var row = minRow; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(minColumn, maxColumn - minColumn + 1)
                .Select(column => (row, column) == droid && panels.GetValueOrDefault((row, column), TileType.Unknown) != TileType.Oxygen
                    ? TileType.Droid
                    : (row, column) == (0, 0)
                        ? TileType.Origin
                        : panels.GetValueOrDefault((row, column), TileType.Unknown)
                )
                .Select(PrintTile)
                .ToList();

            Console.WriteLine(string.Join("", toPrint));
        }

        Console.WriteLine();
    }

    private void PrintPath(Dictionary<(int Row, int Column), TileType> panels, List<(int Row, int Column)> path)
    {
        var minRow = panels.Keys.Min(x => x.Row);
        var maxRow = panels.Keys.Max(x => x.Row);
        var minColumn = panels.Keys.Min(x => x.Column);
        var maxColumn = panels.Keys.Max(x => x.Column);

        for (var row = minRow; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(minColumn, maxColumn - minColumn + 1)
                .Select(column => path.Contains((row, column))
                    ? TileType.Origin
                    : panels.GetValueOrDefault((row, column), TileType.Unknown)
                )
                .Select(PrintTile)
                .ToList();

            Console.WriteLine(string.Join("", toPrint));
        }

        Console.WriteLine();
    }

    private (int Row, int Column) GetTileForCommand((int Row, int Column) currentTile, MovementCommand command)
    {
        return command switch
        {
            MovementCommand.North => (Row: currentTile.Row - 1, Column: currentTile.Column),
            MovementCommand.South => (Row: currentTile.Row + 1, Column: currentTile.Column),
            MovementCommand.West => (Row: currentTile.Row, Column: currentTile.Column - 1),
            MovementCommand.East => (Row: currentTile.Row, Column: currentTile.Column + 1),
        };
    }

    private List<(int row, int Column)> FindShortestPath(Dictionary<(int Row, int Column), TileType> map, (int Row, int Column) source, (int Row, int Column) target)
    {
        var queue = new Queue<(int Row, int Column)>();

        queue.Enqueue(source);
        var explored = new HashSet<(int Row, int Column)>() { source };
        var parents = new Dictionary<(int Row, int Column), (int Row, int Column)>();

        while (queue.Count > 0)
        {
            var toExplore = queue.Dequeue();

            if (toExplore == target)
            {
                break;
            }

            if (!map.ContainsKey(toExplore))
            {
                continue;
            }

            if (map[toExplore] != TileType.Empty && map[toExplore] != TileType.Oxygen)
            {
                continue;
            }

            foreach (var delta in new List<(int Row, int Column)> { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                var neighbour = (Row: toExplore.Row + delta.Row, Column: toExplore.Column + delta.Column);

                if (explored.Contains(neighbour))
                {
                    continue;
                }

                explored.Add(neighbour);
                parents[neighbour] = toExplore;
                queue.Enqueue(neighbour);
            }
        }

        var current = target;
        var result = new List<(int Row, int Column)> { target };

        while (current != source)
        {
            current = parents[current];
            result.Add(current);
        }

        return result;
    }

    private Answer RunRepairSoftware()
    {
        // Stage 1: Exploration.
        // Stage 2: Find your way back to origin and fill holes along the way.
        // Stage 3: Raw calculation to find shortest path.
        var previousTile = (Row: 0, Column: 0);
        var currentTile = (Row: 0, Column: 0);

        var map = new Dictionary<(int Row, int Column), TileType>();
        map[currentTile] = TileType.Empty;

        var inputs = new List<long>();
        var computer = new Computer(_input, inputs);

        var directionRotations = new List<MovementCommand>
        {
            MovementCommand.North,
            MovementCommand.South,
            MovementCommand.West,
            MovementCommand.East,
        };

        while (true)
        {
            MovementCommand? newInput = directionRotations
                .Select(d => (Direction: d, Tile: GetTileForCommand(currentTile, d)))
                .Where(t => !map.ContainsKey(t.Tile) || map[t.Tile] != TileType.Wall)
                .OrderBy(t => t.Tile == previousTile) // Don't go directly back.
                .ThenByDescending(t => !map.ContainsKey(t.Tile) || map[t.Tile] == TileType.Unknown) // Blank or unknowns first
                .ThenByDescending(t => !map.ContainsKey(t.Tile) || map[t.Tile] == TileType.Empty) // Then empty
                .Select(t => t.Direction)
                .FirstOrDefault();

            if (!newInput.HasValue)
            {
                PrintMap(map, currentTile);
                break;
            }

            var targetTile = GetTileForCommand(currentTile, newInput.Value);

            inputs.Add((int)newInput.Value);
            var (returnMode, result) = computer.RunProgram();

            if (returnMode == ReturnMode.Terminate)
            {
                break;
            }

            switch ((StatusCode)result)
            {
                case StatusCode.HitWall:
                    map[targetTile] = TileType.Wall;
                    break;
                case StatusCode.Moved:
                    map[targetTile] = TileType.Empty;
                    previousTile = currentTile;
                    currentTile = targetTile;
                    break;
                case StatusCode.HitOxygen:
                    map[targetTile] = TileType.Oxygen;
                    previousTile = currentTile;
                    currentTile = targetTile;
                    break;
            }

            PrintMap(map, currentTile);

            if (map[targetTile] == TileType.Oxygen)
            {
                var path = FindShortestPath(map, currentTile, (0, 0));
                PrintPath(map, path);
                return path.Count - 1; // This shouldn't actually work unless I happened to find the shortest path, which apparently I did.
            }
        }

        return -1;
    }

    private Answer CalculatePart1Answer()
    {
        return RunRepairSoftware();
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
