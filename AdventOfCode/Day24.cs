namespace AdventOfCode;

public sealed class Day24 : BaseTestableDay
{
    private const int GridSize = 5;
    private readonly HashSet<(int Row, int Column, int Level)> _bugs;

    public Day24() : this(RunMode.Real)
    {
    }

    public Day24(RunMode runMode)
    {
        RunMode = runMode;

        var map = File
            .ReadAllLines(InputFilePath)
            .ToList();

        _bugs = new HashSet<(int Row, int Column, int Level)>();

        for (var row = 0; row < GridSize; row++)
        {
            for (var column = 0; column < GridSize; column++)
            {
                if (map[row][column] == '#')
                {
                    _bugs.Add((row, column, 0));
                }
            }
        }
    }

    private string GetBugsString(HashSet<(int Row, int Column, int Level)> bugs)
    {
        return string.Join(
            ", ",
            bugs
                .OrderBy(t => t.Row)
                .ThenBy(t => t.Column)
                .ThenBy(t => t.Level)
                .Select(t => $"({t.Row}, {t.Column}, {t.Level})")
        );
    }

    private int GetBiodiversityRating(int row, int column)
    {
        return (int)Math.Pow(2.0, row * GridSize + column);
    }

    private HashSet<(int Row, int Column, int Level)> Simulate(
        List<(int Row, int Column)> singleLevelSpots,
        Dictionary<(int Row, int Column), List<(int Row, int Column, int Level)>> allDeltas,
        int minuteLimit
    )
    {
        var bugs = _bugs.ToHashSet();

        var history = new HashSet<string>();

        for (var minute = 0; minute < minuteLimit; minute++)
        {
            var bugString = GetBugsString(bugs);

            if (!history.Add(bugString)) // This should only get triggered in #1
            {
                return bugs;
            }

            var deadBugs = new List<(int Row, int Column, int Level)>();
            var infestedBugs = new List<(int Row, int Column, int Level)>();

            var minLevel = bugs.Min(b => b.Level);
            var maxLevel = bugs.Max(b => b.Level);

            var allSpots = Enumerable
                .Range(minLevel - 1, maxLevel - minLevel + 3)
                .SelectMany(l => singleLevelSpots.Select(s => (s.Row, s.Column, Level: l)))
                .ToList();

            foreach (var spot in allSpots)
            {
                var adjacentBugs = allDeltas[(spot.Row, spot.Column)]
                    .Select(d => (spot.Row + d.Row, spot.Column + d.Column, spot.Level + d.Level))
                    .Count(n => bugs.Contains(n));

                if (bugs.Contains(spot) && adjacentBugs != 1)
                {
                    deadBugs.Add(spot);
                }
                else if (!bugs.Contains(spot) && 1 <= adjacentBugs && adjacentBugs <= 2)
                {
                    infestedBugs.Add(spot);
                }
            }

            bugs.UnionWith(infestedBugs);
            bugs.ExceptWith(deadBugs);
        }

        return bugs;
    }

    private Answer CalculatePart1Answer()
    {
        var singleLevelSpots = Enumerable.Range(0, GridSize)
            .SelectMany(row => Enumerable.Range(0, GridSize).Select(column => (Row: row, Column: column)))
            .ToList();

        var deltas = new List<(int Row, int Column)> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var allDeltas = singleLevelSpots
            .ToDictionary(
                s => s,
                s => deltas
                    .Where(d => 0 <= s.Row + d.Row && s.Row + d.Row < GridSize && 0 <= s.Column + d.Column && s.Column + d.Column < GridSize)
                    .Select(d => (d.Row, d.Column, Level: 0))
                    .ToList()
            );

        var bugs = Simulate(singleLevelSpots, allDeltas, minuteLimit: 200);

        return bugs.Select(t => GetBiodiversityRating(t.Row, t.Column)).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        var singleLevelSpots = Enumerable.Range(0, GridSize)
            .SelectMany(row => Enumerable.Range(0, GridSize).Select(column => (Row: row, Column: column)))
            .Where(t => t != (2, 2))
            .ToList();

        var deltas = new List<(int Row, int Column)> { (0, 1), (0, -1), (1, 0), (-1, 0) };
        var allDeltas = singleLevelSpots
            .ToDictionary(
                s => s,
                s => deltas
                    .Where(d => (s.Row + d.Row, s.Column + d.Column) != (2, 2))
                    .Where(d => 0 <= s.Row + d.Row && s.Row + d.Row < GridSize && 0 <= s.Column + d.Column && s.Column + d.Column < GridSize)
                    .Select(d => (d.Row, d.Column, Level: 0))
                    .ToList()
            );

        // Manually add neighbours that cross levels
        // One level up
        allDeltas[(0, 0)].Add((1, 2, 1));
        allDeltas[(0, 1)].Add((1, 1, 1));
        allDeltas[(0, 2)].Add((1, 0, 1));
        allDeltas[(0, 3)].Add((1, -1, 1));
        allDeltas[(0, 4)].Add((1, -2, 1));

        allDeltas[(4, 0)].Add((-1, 2, 1));
        allDeltas[(4, 1)].Add((-1, 1, 1));
        allDeltas[(4, 2)].Add((-1, 0, 1));
        allDeltas[(4, 3)].Add((-1, -1, 1));
        allDeltas[(4, 4)].Add((-1, -2, 1));

        allDeltas[(0, 0)].Add((2, 1, 1));
        allDeltas[(0, 4)].Add((2, -1, 1));

        allDeltas[(1, 0)].Add((1, 1, 1));
        allDeltas[(1, 4)].Add((1, -1, 1));

        allDeltas[(2, 0)].Add((0, 1, 1));
        allDeltas[(2, 4)].Add((0, -1, 1));

        allDeltas[(3, 0)].Add((-1, 1, 1));
        allDeltas[(3, 4)].Add((-1, -1, 1));

        allDeltas[(4, 0)].Add((-2, 1, 1));
        allDeltas[(4, 4)].Add((-2, -1, 1));

        // One level down
        allDeltas[(1, 2)].Add((-1, -2, -1));
        allDeltas[(1, 2)].Add((-1, -1, -1));
        allDeltas[(1, 2)].Add((-1, 0, -1));
        allDeltas[(1, 2)].Add((-1, 1, -1));
        allDeltas[(1, 2)].Add((-1, 2, -1));

        allDeltas[(3, 2)].Add((1, -2, -1));
        allDeltas[(3, 2)].Add((1, -1, -1));
        allDeltas[(3, 2)].Add((1, 0, -1));
        allDeltas[(3, 2)].Add((1, 1, -1));
        allDeltas[(3, 2)].Add((1, 2, -1));

        allDeltas[(2, 1)].Add((-2, -1, -1));
        allDeltas[(2, 1)].Add((-1, -1, -1));
        allDeltas[(2, 1)].Add((0, -1, -1));
        allDeltas[(2, 1)].Add((1, -1, -1));
        allDeltas[(2, 1)].Add((2, -1, -1));

        allDeltas[(2, 3)].Add((-2, 1, -1));
        allDeltas[(2, 3)].Add((-1, 1, -1));
        allDeltas[(2, 3)].Add((0, 1, -1));
        allDeltas[(2, 3)].Add((1, 1, -1));
        allDeltas[(2, 3)].Add((2, 1, -1));

        var bugs = Simulate(singleLevelSpots, allDeltas, minuteLimit: this.RunMode == RunMode.Test ? 10 : 200);
        return bugs.Count;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
