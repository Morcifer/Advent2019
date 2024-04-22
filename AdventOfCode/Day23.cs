namespace AdventOfCode;

public sealed class Day23 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day23() : this(RunMode.Real)
    {
    }

    public Day23(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }


    private Answer CalculatePart1Answer()
    {
        var inputs = Enumerable.Range(0, 50).Select(i => new List<long>() { i }).ToList();
        var computers = inputs.Select(i => new Computer(_input, i)).ToList();
        var packets = inputs.Select(_ => new Queue<(long X, long Y)>()).ToList();

        while (packets.Count > 0)
        {
            for (var i = 0; i < 50; i++)
            {
                var output = computers[i].RunProgram();

                if (output.Item1 == ReturnMode.Input)
                {
                    if (packets[i].Count > 0)
                    {
                        var packet = packets[i].Dequeue();
                        inputs[i].Add(packet.X);
                        inputs[i].Add(packet.Y);
                    }
                    else
                    {
                        inputs[i].Add(-1);
                    }
                }
                else if (output.Item1 == ReturnMode.Output)
                {
                    var target = output.Item2.Value;
                    var x = computers[i].RunProgram().Item2.Value;
                    var y = computers[i].RunProgram().Item2.Value;

                    if (target == 255)
                    {
                        return y;
                    }

                    packets[(int)target].Enqueue((x, y));
                }
            }
        }

        return -1;
    }

    private Answer CalculatePart2Answer()
    {
        var inputs = Enumerable.Range(0, 50).Select(i => new List<long>() { i }).ToList();
        var computers = inputs.Select(i => new Computer(_input, i)).ToList();
        var packets = inputs.Select(_ => new Queue<(long X, long Y)>()).ToList();

        (long X, long Y) nat = (-1, -1);
        var idle = inputs.Select(_ => false).ToList();

        var natHistory = new HashSet<long>();

        while (packets.Count > 0)
        {
            if (idle.All(x => x)) // All of the computers are idle
            {
                if (!natHistory.Add(nat.Y))
                {
                    return nat.Y;
                }

                packets[0].Enqueue(nat);
            }

            idle = inputs.Select(_ => false).ToList();

            for (var i = 0; i < 50; i++)
            {
                var output = computers[i].RunProgram();

                if (output.Item1 == ReturnMode.Input)
                {
                    if (packets[i].Count > 0)
                    {
                        var packet = packets[i].Dequeue();
                        inputs[i].Add(packet.X);
                        inputs[i].Add(packet.Y);
                    }
                    else
                    {
                        idle[i] = true;
                        inputs[i].Add(-1);
                    }
                }
                else if (output.Item1 == ReturnMode.Output)
                {
                    var target = output.Item2.Value;
                    var x = computers[i].RunProgram().Item2.Value;
                    var y = computers[i].RunProgram().Item2.Value;

                    if (target == 255)
                    {
                        nat = (x, y);
                    }
                    else
                    {
                        packets[(int)target].Enqueue((x, y));
                    }
                }
            }
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
