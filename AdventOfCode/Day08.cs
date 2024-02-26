using System.Reflection.Emit;

namespace AdventOfCode;

public sealed class Day08 : BaseTestableDay
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<int> _input;

    public Day08() : this(RunMode.Real)
    {
    }

    public Day08(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)[0]
            .Select(c => int.Parse(c.ToString()))
            .ToList();

        _width = runMode == RunMode.Test ? 2 : 25;
        _height = runMode == RunMode.Test ? 2 : 6;
    }

    private List<int[]> GetLayers()
    {
        // There's a more performent way to write this, but this one is much more readable.
        return Enumerable.Range(0, _input.Count / (_width * _height))
            .Select(l => _input.Skip(l * _width * _height).Take(_width * _height).ToArray())
            .ToList();
    }

    private Answer CalculatePart1Answer()
    {
        var layers = GetLayers();
        var targetLayer = layers.MinBy(layer => layer.Count(x => x == 0));
        return targetLayer.Count(x => x == 1) * targetLayer.Count(x => x == 2);
    }

    private Answer CalculatePart2Answer()
    {
        var layers = GetLayers();

        var rulingLayer = new int[_width * _height];

        foreach (var layer in layers.Reversed()) // From the bottom up.
        {
            rulingLayer = layer.Select((pixel, index) => pixel == 2 ? rulingLayer[index] : pixel).ToArray();
        }

        for (int h = 0; h < _height; h++)
        {
            var row = rulingLayer.Skip(_width * h).Take(_width).ToList();
            var textRow = row.Select(x => x == 1 ? "*" : " ").ToList();

            Console.WriteLine(string.Join("", textRow));
        }

        return "LEGJY";
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
