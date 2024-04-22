namespace AdventOfCode;

public sealed class Day24 : BaseTestableDay
{
    private const int GridSize = 5;
    private readonly List<string> _input;

    public Day24() : this(RunMode.Real)
    {
    }

    public Day24(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .ToList();
    }

    private int GetSmooshedCoordinate(int row, int column)
    {
        return row * GridSize + column;
    }

    private Answer CalculatePart1Answer()
    {
        var deltas = new List<(int Row, int Column)> { (0, 1), (0, -1), (1, 0), (-1, 0) };

        var smooshed = string.Join("", _input);
        var history = new HashSet<string>();

        while (!history.Contains(smooshed))
        {
            history.Add(smooshed);

            var deadBugs = new List<int>();
            var infestedBugs = new List<int>();

            for (var row = 0; row < GridSize; row++)
            {
                for (var column = 0; column < GridSize; column++)
                {
                    var adjacentBugs = deltas
                        .Select(t => (Row: row + t.Row, Column: column + t.Column))
                        .Where(t => 0 <= t.Row && t.Row < GridSize && 0 <= t.Column && t.Column < GridSize)
                        .Select(t => GetSmooshedCoordinate(t.Row, t.Column))
                        .Count(c => smooshed[c] == '#');

                    var thisSmooshedCoordinate = GetSmooshedCoordinate(row, column);

                    if (smooshed[thisSmooshedCoordinate] == '#' && adjacentBugs != 1)
                    {
                        deadBugs.Add(thisSmooshedCoordinate);
                    }
                    else if (smooshed[thisSmooshedCoordinate] == '.' && 1 <= adjacentBugs && adjacentBugs <= 2)
                    {
                        infestedBugs.Add(thisSmooshedCoordinate);
                    }
                }
            }

            var newSmooshed = smooshed.ToCharArray();

            foreach (var dead in deadBugs)
            {
                newSmooshed[dead] = '.';
            }

            foreach (var infested in infestedBugs)
            {
                newSmooshed[infested] = '#';
            }

            smooshed = string.Join("", newSmooshed);
        }

        return smooshed.Select((c, i) => c == '#' ? (int)Math.Pow(2.0, i) : 0).Sum();
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
