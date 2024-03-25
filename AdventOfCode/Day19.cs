namespace AdventOfCode;

using GridSpot = (int Row, int Column);

public sealed class Day19 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day19() : this(RunMode.Real)
    {
    }

    public Day19(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    private char InvestigateSpot(GridSpot spot)
    {
        var computerInputs = new List<long>();
        var computer = new Computer(_input, computerInputs);

        computerInputs.Add(spot.Column);
        computerInputs.Add(spot.Row);

        var results = computer.RunProgramToTermination();
        var result = results[^1];

        return result == 0 ? '.' : '#';
    }

    private char[,] SearchFirstPartOfGrid()
    {
        var known = new char[50, 50];

        for (var row = 0; row < 50; row++)
        {
            for (var column = 0; column < 50; column++)
            {
                var found = InvestigateSpot((row, column));
                known[row, column] = found;
                //Console.WriteLine($"Grid spot {location} is {found}");
            }
        }

        return known;
    }

    private Answer CalculatePart1Answer()
    {
        var known = SearchFirstPartOfGrid();

        var counter = 0;

        for (var i = 0; i < 50; i++)
        {
            var row = new List<char>();

            for (var j = 0; j < 50; j++)
            {
                if (known[i, j] == '#')
                {
                    counter++;
                }

                row.Add(known[i, j]);
            }

            //Console.WriteLine($"{string.Join("", row)}");
        }

        return counter;
    }

    private Answer CalculatePart2Answer()
    {
        var known = SearchFirstPartOfGrid();

        var row = 49;

        var first = Enumerable.Range(0, 50).Where(c => known[row, c] == '#').Min();
        var last = Enumerable.Range(0, 50).Where(c => known[row, c] == '#').Max();

        var rowRanges = new Dictionary<int, (int First, int Last)>() { { row++, (first, last) }, };

        for (;;row++)
        {
            first = Enumerable.Range(-2, 5).Select(d => first + d).Where(column => InvestigateSpot((row, column)) == '#').Min();
            last = Enumerable.Range(-2, 5).Select(d => last + d).Where(column => InvestigateSpot((row, column)) == '#').Max();
            rowRanges[row] = (first, last);

            if (row >= 200 && rowRanges[row - 99].First <= first + 99 && first + 99 <= rowRanges[row - 99].Last)
            {
                return 10000 * first + (row - 99);
            }

            //Console.WriteLine($"Range in row {row} is ({first}, {last}) (width {last - first + 1})");
        }
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
