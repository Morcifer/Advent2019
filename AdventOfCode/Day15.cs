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
    private readonly Computer _computer;
    private readonly List<long> _computerInputs;

    private (int Row, int Column) _currentTile;
    private Dictionary<(int Row, int Column), TileType> _map;

    public Day15() : this(RunMode.Real)
    {
    }

    public Day15(RunMode runMode)
    {
        RunMode = runMode;

        var program = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();

        _computerInputs = new List<long>();
        _computer = new Computer(program, _computerInputs);

        _currentTile = (Row: 0, Column: 0);
        _map = new Dictionary<(int Row, int Column), TileType>();
        _map[_currentTile] = TileType.Empty;
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

    private void PrintMap((int Row, int Column) droid)
    {
        var minRow = _map.Keys.Min(x => x.Row);
        var maxRow = _map.Keys.Max(x => x.Row);
        var minColumn = _map.Keys.Min(x => x.Column);
        var maxColumn = _map.Keys.Max(x => x.Column);

        for (var row = minRow; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(minColumn, maxColumn - minColumn + 1)
                .Select(column => (row, column) == droid && _map.GetValueOrDefault((row, column), TileType.Unknown) != TileType.Oxygen
                    ? TileType.Droid
                    : (row, column) == (0, 0)
                        ? TileType.Origin
                        : _map.GetValueOrDefault((row, column), TileType.Unknown)
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

    private MovementCommand GetCommandForTileMove((int Row, int Column) currentTile, (int Row, int Column) nextTile)
    {
        return (nextTile.Row - currentTile.Row, nextTile.Column - currentTile.Column) switch
        {
            (-1, 0) => MovementCommand.North,
            (1, 0) => MovementCommand.South,
            (0, -1) => MovementCommand.West,
            (0, 1) => MovementCommand.East,
            _ => throw new ArgumentException("Can't move from current to next in one go.")
        };
    }

    private List<(int row, int Column)> FindShortestPath((int Row, int Column) source, (int Row, int Column) target)
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

            if (!_map.ContainsKey(toExplore))
            {
                continue;
            }

            if (_map[toExplore] != TileType.Empty && _map[toExplore] != TileType.Oxygen)
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

        return result.Reversed().ToList();
    }

    private void FindOxygen()
    {
        // Stage 1: Exploration.
        // Stage 2: Find your way back to origin and fill holes along the way. -- not needed in the end.
        // Stage 3: Raw calculation to find shortest path from origin to oxygen.
        var previousTile = (Row: 0, Column: 0);

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
                .Select(d => (Direction: d, Tile: GetTileForCommand(_currentTile, d)))
                .Where(t => !_map.ContainsKey(t.Tile) || _map[t.Tile] != TileType.Wall)
                .OrderBy(t => t.Tile == previousTile) // Don't go directly back.
                .ThenByDescending(t => !_map.ContainsKey(t.Tile) || _map[t.Tile] == TileType.Unknown) // Blank or unknowns first
                .ThenByDescending(t => !_map.ContainsKey(t.Tile) || _map[t.Tile] == TileType.Empty) // Then empty
                .Select(t => t.Direction)
                .FirstOrDefault();

            if (!newInput.HasValue)
            {
                PrintMap(_currentTile);
                break;
            }

            var targetTile = GetTileForCommand(_currentTile, newInput.Value);

            _computerInputs.Add((int)newInput.Value);
            var (returnMode, result) = _computer.RunProgram();

            if (returnMode == ReturnMode.Terminate)
            {
                break;
            }

            switch ((StatusCode)result)
            {
                case StatusCode.HitWall:
                    _map[targetTile] = TileType.Wall;
                    break;
                case StatusCode.Moved:
                    _map[targetTile] = TileType.Empty;
                    previousTile = _currentTile;
                    _currentTile = targetTile;
                    break;
                case StatusCode.HitOxygen:
                    _map[targetTile] = TileType.Oxygen;
                    previousTile = _currentTile;
                    _currentTile = targetTile;
                    break;
            }

            //PrintMap(map, currentTile);

            if (_map[targetTile] == TileType.Oxygen)
            {
                return;
            }
        }

        throw new ApplicationException("Failed to find the oxygen, I am a failure");
;    }

    private Answer RunRepairSoftware()
    {
        FindOxygen();

        var path = FindShortestPath(_currentTile, (0, 0));

        return path.Count - 1; // This shouldn't actually work unless I happened to find the shortest path, which apparently I did.
}

    private Answer CalculatePart1Answer()
    {
        return RunRepairSoftware();
    }

    private Answer CalculatePart2Answer()
    {
        // In this particular case, note that the computer state is inherited from part 1!
        // So now we can start flood-filling from our current location, which is where the oxygen is.
        var queue = new Queue<((int Row, int Column) Tile, int Distance)>();
        queue.Enqueue((_currentTile, 0));

        var explored = new HashSet<(int Row, int Column)>();
        var maxLevel = 0;

        while (queue.Count > 0)
        {
            var (toExplore, level) = queue.Dequeue();

            if (explored.Contains(toExplore))
            {
                continue;
            }

            explored.Add(toExplore);

            if (!_map.ContainsKey(toExplore))
            {
                //PrintMap(map, currentLocation);

                // There's a gap, so go there with the droid to fill it up.
                var path = FindShortestPath(_currentTile, toExplore);

                foreach (var fromTo in path.Windows())
                {
                    _computerInputs.Add((long)GetCommandForTileMove(fromTo.Item1, fromTo.Item2));
                    var (returnMode, result) = _computer.RunProgram();

                    if (returnMode == ReturnMode.Terminate)
                    {
                        throw new ApplicationException();
                    }

                    if (returnMode == ReturnMode.Input)
                    {
                        throw new ApplicationException();
                    }

                    if (fromTo.Item2 == path[^1]) // LAST!
                    {
                        _currentTile = fromTo.Item1;
                        var targetTile = fromTo.Item2;

                        switch ((StatusCode)result)
                        {
                            case StatusCode.HitWall:
                                _map[targetTile] = TileType.Wall;
                                break;
                            case StatusCode.Moved:
                                _map[targetTile] = TileType.Empty;
                                _currentTile = targetTile;
                                break;
                            case StatusCode.HitOxygen:
                                _map[targetTile] = TileType.Oxygen;
                                _currentTile = targetTile;
                                break;
                        }
                    }
                    else if ((StatusCode)result == StatusCode.HitWall)
                    {
                        throw new ApplicationException();
                    }
                }

                //PrintMap(map, currentLocation);
            }

            if (_map[toExplore] == TileType.Wall)
            {
                continue;
            }

            maxLevel = Math.Max(maxLevel, level);

            foreach (var delta in new List<(int Row, int Column)> { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                var neighbour = (Row: toExplore.Row + delta.Row, Column: toExplore.Column + delta.Column);
                queue.Enqueue((neighbour, level + 1));
            }
        }

        // There seems to be more than 1 oxygen source. :/
        var oxygenSpots = _map.Where(kvp => kvp.Value == TileType.Oxygen).Select(kvp => kvp.Key).ToList();

        if (oxygenSpots.Count == 1)
        {
            return maxLevel;
        }

        PrintMap(_currentTile);
        queue = new Queue<((int Row, int Column) Tile, int Distance)>();
        oxygenSpots.ForEach(s => queue.Enqueue((s, 0)));

        explored = new HashSet<(int Row, int Column)>();
        maxLevel = 0;

        while (queue.Count > 0)
        {
            var (toExplore, level) = queue.Dequeue();

            if (explored.Contains(toExplore))
            {
                continue;
            }

            explored.Add(toExplore);

            if (_map[toExplore] == TileType.Wall)
            {
                continue;
            }

            maxLevel = Math.Max(maxLevel, level);

            foreach (var delta in new List<(int Row, int Column)> { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                var neighbour = (Row: toExplore.Row + delta.Row, Column: toExplore.Column + delta.Column);
                queue.Enqueue((neighbour, level + 1));
            }
        }

        return maxLevel;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
