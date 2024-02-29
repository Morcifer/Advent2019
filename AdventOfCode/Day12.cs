using MathNet.Numerics;
using MoreLinq;
using Vector3D = System.Collections.Generic.List<int>;

namespace AdventOfCode;

public sealed class Day12 : BaseTestableDay
{
   private readonly List<Vector3D> _input;

    public Day12() : this(RunMode.Real)
    {
    }

    public Day12(RunMode runMode)
    {
        RunMode = runMode;

        _input = new List<Vector3D>();

        switch (runMode)
        {
            case RunMode.Test:
                _input.Add([-1, 0, 2]);
                _input.Add([2, -10, -7]);
                _input.Add([4, -8, 8]);
                _input.Add([3, 5, -1]);
                break;

            case RunMode.Test2:
                _input.Add([-8, -10, 0]);
                _input.Add([5, 5, 10]);
                _input.Add([2, -7, 3]);
                _input.Add([9, -8, -3]);
                break;

            case RunMode.Real:
                _input.Add([-3, 15, -11]);
                _input.Add([3, 13, -19]);
                _input.Add([-13, 18, -2]);
                _input.Add([6, 0, -1]);
                break;
        }
    }

    private Answer SimulateGravity(int numberOfSteps, bool firstPart)
    {
        var positions = _input.ToList();
        var velocities = _input.Select(_ => new Vector3D { 0, 0, 0 }).ToList();

        var potential = _input.Select(_ => 0).ToList();
        var kinetic = _input.Select(_ => 0).ToList();

        var axisHistory = Enumerable.Range(0, 3).Select(_ => new Dictionary<(int, int, int, int, int, int, int, int), int>()).ToList();
        var axisHistoryRepeats = Enumerable.Range(0, 3).Select(_ => 0).ToList();

        for (var step = 0; step < numberOfSteps; step++)
        {
            // Check history
            foreach (var axis in Enumerable.Range(0, 3))
            {
                var keyList = positions.Select(p => p[axis]).Concat(velocities.Select(v => v[axis])).ToList();
                var key = (keyList[0], keyList[1], keyList[2], keyList[3], keyList[4], keyList[5], keyList[6], keyList[7]);

                if (axisHistoryRepeats[axis] == 0 && axisHistory[axis].ContainsKey(key))
                {
                    //Console.WriteLine($"History repeats itself for axis {axis} at {step}");
                    axisHistoryRepeats[axis] = step - axisHistory[axis][key];
                }

                axisHistory[axis][key] = step;
            }

            if (!firstPart && axisHistoryRepeats.All(h => h > 0))
            {
                return Euclid.LeastCommonMultiple(axisHistoryRepeats[0], axisHistoryRepeats[1], axisHistoryRepeats[2]);
            }

            // Update velocities
                foreach (var pair in Enumerable.Range(0, 4).Subsets(subsetSize: 2))
            {
                var first = pair[0];
                var second = pair[1];

                var firstPosition = positions[first];
                var secondPosition = positions[second];

                var diffs = firstPosition.Zip(secondPosition).Select(t => t.First.CompareTo(t.Second)).ToList();

                velocities[first] = velocities[first].Zip(diffs).Select(t => t.First - t.Second).ToList();
                velocities[second] = velocities[second].Zip(diffs).Select(t => t.First + t.Second).ToList();
            }

            // Update positions
            foreach (var moon in Enumerable.Range(0, 4))
            {
                positions[moon] = positions[moon].Zip(velocities[moon]).Select(t => t.First + t.Second).ToList();
            }

            potential = positions.Select(m => m.Sum(Math.Abs)).ToList();
            kinetic = velocities.Select(m => m.Sum(Math.Abs)).ToList();

            if (firstPart)
            {
                continue;
            }
        }

        return Enumerable.Range(0, 4).Select(moon => potential[moon] * kinetic[moon]).Sum();
    }

    private Answer CalculatePart1Answer()
    {
        return SimulateGravity(1000, true);
    }

    private Answer CalculatePart2Answer()
    {
        return SimulateGravity(1000000, false);
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
