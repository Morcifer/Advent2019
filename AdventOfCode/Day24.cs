namespace AdventOfCode;

public sealed class Day24 : BaseTestableDay
{
    private const int GridSize = 5;
    private readonly HashSet<(int Row, int Column)> _bugs;

    public Day24() : this(RunMode.Real)
    {
    }

    public Day24(RunMode runMode)
    {
        RunMode = runMode;

        var map = File
            .ReadAllLines(InputFilePath)
            .ToList();

        _bugs = new HashSet<(int Row, int Column)>();

        for (var row = 0; row < GridSize; row++)
        {
            for (var column = 0; column < GridSize; column++)
            {
                if (map[row][column] == '#')
                {
                    _bugs.Add((row, column));
                }
            }
        }
    }

    private string GetBugsString()
    {
        return string.Join(", ", _bugs.OrderBy(t => t.Row).ThenBy(t => t.Column).Select(t => $"({t.Row}, {t.Column})"));
    }

    private int GetBiodiversityRating(int row, int column)
    {
        return (int)Math.Pow(2.0, row * GridSize + column);
    }

    private Answer CalculatePart1Answer()
    {
        var deltas = new List<(int Row, int Column)> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var history = new HashSet<string>();

        while (!history.Contains(GetBugsString()))
        {
            history.Add(GetBugsString());

            var deadBugs = new List<(int Row, int Column)>();
            var infestedBugs = new List<(int Row, int Column)>();

            for (var row = 0; row < GridSize; row++)
            {
                for (var column = 0; column < GridSize; column++)
                {
                    var adjacentBugs = deltas
                        .Select(t => (Row: row + t.Row, Column: column + t.Column))
                        .Count(n => _bugs.Contains((n.Row, n.Column)));

                    if (_bugs.Contains((row, column)) && adjacentBugs != 1)
                    {
                        deadBugs.Add((row, column));
                    }
                    else if (!_bugs.Contains((row, column)) && 1 <= adjacentBugs && adjacentBugs <= 2)
                    {
                        infestedBugs.Add((row, column));
                    }
                }
            }

            _bugs.UnionWith(infestedBugs);
            _bugs.ExceptWith(deadBugs);
        }

        return _bugs.Select(t => GetBiodiversityRating(t.Row, t.Column)).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
