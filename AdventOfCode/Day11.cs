using System.Diagnostics;

namespace AdventOfCode;

public sealed class Day11 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day11() : this(RunMode.Real)
    {
    }

    public Day11(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    private (HashSet<(int, int)>, HashSet<(int, int)>) RunPaintingSoftware(bool whiteBegins)
    {
        var allPanels = new HashSet<(int, int)>();;
        var whitePanels = new HashSet<(int, int)>();
        var currentPanel = (Row: 0, Column: 0);
        var currentAngle = 0; // Up.

        if (whiteBegins)
        {
            whitePanels.Add(currentPanel);
        }

        var inputs = new List<long>();
        var computer = new Computer(_input, inputs);

        while (true)
        {
            // Update input for current location.
            inputs.Add(whitePanels.Contains(currentPanel) ? 1 : 0); // 0 if panel is black, 1 if it's white.

            // First output - color to paint.
            var (returnMode, color) = computer.RunProgram(); // 0 = black, 1 = white.

            if (returnMode == ReturnMode.Terminate)
            {
                break;
            }

            switch (color)
            {
                case 0 when whitePanels.Contains(currentPanel):
                    whitePanels.Remove(currentPanel);
                    break;
                case 1:
                    whitePanels.Add(currentPanel);
                    break;
            }

            allPanels.Add(currentPanel);

            // Second output - direction to turn.
            var (_, direction) = computer.RunProgram(); // 0 = left 90 degrees, 1 = right 90 degrees.

            switch (direction)
            {
                case 0:
                    currentAngle -= 90;

                    if (currentAngle < 0)
                    {
                        currentAngle += 360;
                    }
                    break;
                case 1:
                    currentAngle += 90;
                    currentAngle %= 360;
                    break;
            }

            // The move forward.
            currentPanel = currentAngle switch
            {
                0 => (currentPanel.Row - 1, currentPanel.Column),
                90 => (currentPanel.Row, currentPanel.Column + 1),
                180 => (currentPanel.Row + 1, currentPanel.Column),
                270 => (currentPanel.Row, currentPanel.Column - 1),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        return (allPanels, whitePanels);
    }

    private Answer CalculatePart1Answer()
    {
        var (allPanels, _) = RunPaintingSoftware(whiteBegins: false);
        return allPanels.Count;
    }

    private Answer CalculatePart2Answer()
    {
        var (_, whitePanels) = RunPaintingSoftware(whiteBegins: true);
        var minRow = whitePanels.Min(x => x.Item1);
        var maxRow = whitePanels.Max(x => x.Item1);
        var minColumn = whitePanels.Min(x => x.Item2);
        var maxColumn = whitePanels.Max(x => x.Item2);

        Debug.Assert(minRow >= 0 && minColumn >= 0); // Smaller isn't supported, the code will have to be updated.

        for (var row = 0; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(0, maxColumn + 1)
                .Select(column => whitePanels.Contains((row, column)) ? "*" : " ")
                .ToList();

            Console.WriteLine(string.Join("", toPrint));
        }

        return "JKZLZJBH";
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
