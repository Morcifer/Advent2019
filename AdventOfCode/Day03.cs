namespace AdventOfCode;

public enum Direction
{
    Up,
    Down,
    Right,
    Left,
}

public sealed class Day03 : BaseTestableDay
{
    private readonly List<List<(Direction Direction, int Steps)>> _input;

    public Day03() : this(RunMode.Real)
    {
    }

    public Day03(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .Select(line => line.Split(',').Select(step => (
                    ConvertStringToDirection(step[0]),
                    int.Parse(step[1..])
                ))
                .ToList()
            )
            .ToList();
    }

    private Direction ConvertStringToDirection(char direction)
    {
        switch (direction)
        {
            case 'U':
                return Direction.Up;
            case 'D':
                return Direction.Down;
            case 'R':
                return Direction.Right;
            case 'L':
                return Direction.Left;
            default:
                throw new ArgumentException("Invalid value for Direction", nameof(Direction));
        }
    }

    private List<(int, int)> GetPath(List<(Direction Direction, int Steps)> pathSteps)
    {
        var currentPoint = (0, 0);
        var path = new List<(int, int)>() { currentPoint };

        foreach (var pathStep in pathSteps)
        {
            var delta = (pathStep.Direction) switch
            {
                Direction.Up => (0, 1),
                Direction.Down => (0, -1),
                Direction.Right => (1, 0),
                Direction.Left => (-1, 0),
                _ => throw new ArgumentException("Invalid value for OpCode", nameof(OpCode)),
            };

            foreach (var _ in Enumerable.Range(0, pathStep.Steps))
            {
                currentPoint = (currentPoint.Item1 + delta.Item1, currentPoint.Item2 + delta.Item2);
                path.Add(currentPoint);
            }
        }

        return path;
    }


    private Answer FindClosestIntersection()
    {
        var wire1Path = GetPath(_input[0]).ToHashSet();
        var wire2Path = GetPath(_input[1]).ToHashSet();

        return wire1Path
            .Intersect(wire2Path)
            .Select(point => Math.Abs(point.Item1) + Math.Abs(point.Item2))
            .Where(d => d != 0)
            .Min();
    }

    private Answer FindShortestLatencyIntersection()
    {
        var wire1Path = GetPath(_input[0]);
        var wire2Path = GetPath(_input[1]);

        var intersections = wire1Path.ToHashSet()
            .Intersect(wire2Path.ToHashSet())
            .Where(point => point.Item1 != 0 || point.Item2 != 0)
            .ToList();

        var wire1Latencies = wire1Path.Enumerate().Where(x => intersections.Contains(x.Value))
            .ToDictionary(x => x.Value, x => x.Index);
        var wire2Latencies = wire2Path.Enumerate().Where(x => intersections.Contains(x.Value))
            .ToDictionary(x => x.Value, x => x.Index);
        return intersections
            .Select(point => wire1Latencies[point] + wire2Latencies[point])
            .Min();
    }


    public override ValueTask<string> Solve_1() => new(FindClosestIntersection());

    public override ValueTask<string> Solve_2() => new(FindShortestLatencyIntersection());
}
