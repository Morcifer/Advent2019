using MoreLinq;
using Vector3D = (int X, int Y, int Z);

namespace AdventOfCode;

public sealed class Day12 : BaseTestableDay
{
    // TODO 1: use a sparse representation, even though it looks like it's unnecessary.
    // TODO 2: Introduce grid coordinate struct that has (x, y) and (row, column), because this is silly!
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
                _input.Add(new Vector3D(-1, 0, 2));
                _input.Add(new Vector3D(2, -10, -7));
                _input.Add(new Vector3D(4, -8, 8));
                _input.Add(new Vector3D(3, 5, -1));
                break;

            case RunMode.Real:
                _input.Add(new Vector3D(-3, 15, -11));
                _input.Add(new Vector3D(3, 13, -19));
                _input.Add(new Vector3D(-13, 18, -2));
                _input.Add(new Vector3D(6, 0, -1));
                break;
        }
    }

    private Answer SimulateGravity(int numberOfSteps)
    {
        var positions = _input.ToList();
        var velocities = _input.Select(_ => new Vector3D(0, 0, 0)).ToList();

        var potential = _input.Select(_ => 0).ToList();
        var kinetic = _input.Select(_ => 0).ToList();

        for (var step = 0; step < numberOfSteps; step++)
        {
            foreach (var pair in Enumerable.Range(0, 4).Subsets(subsetSize: 2))
            {
                var first = pair[0];
                var second = pair[1];

                var firstPosition = positions[first];
                var secondPosition = positions[second];

                var diffs = new Vector3D(
                    firstPosition.X.CompareTo(secondPosition.X),
                    firstPosition.Y.CompareTo(secondPosition.Y),
                    firstPosition.Z.CompareTo(secondPosition.Z)
                );

                velocities[first] = new Vector3D(
                    velocities[first].X - diffs.X,
                    velocities[first].Y - diffs.Y,
                    velocities[first].Z - diffs.Z
                );

                velocities[second] = new Vector3D(
                    velocities[second].X + diffs.X,
                    velocities[second].Y + diffs.Y,
                    velocities[second].Z + diffs.Z
                );
            }

            foreach (var moon in Enumerable.Range(0, 4))
            {
                positions[moon] = new Vector3D(
                    positions[moon].X + velocities[moon].X,
                    positions[moon].Y + velocities[moon].Y,
                    positions[moon].Z + velocities[moon].Z
                );
            }

            potential = positions.Select(m => Math.Abs(m.X) + Math.Abs(m.Y) + Math.Abs(m.Z)).ToList();
            kinetic = velocities.Select(m => Math.Abs(m.X) + Math.Abs(m.Y) + Math.Abs(m.Z)).ToList();

            //Console.WriteLine($"After {step} steps");
            //foreach (var moon in Enumerable.Range(0, 4))
            //{
            //    Console.WriteLine(
            //        $"Moon {moon}: position ({positions[moon].X}, {positions[moon].Y}, {positions[moon].Z}) " +
            //        $"with velocity ({velocities[moon].X}, {velocities[moon].Y}, {velocities[moon].Z}), " +
            //        $"and energies potential={potential[moon]}, kinetic={kinetic[moon]}"
            //    );
            //}
        }

        return Enumerable.Range(0, 4).Select(moon => potential[moon] * kinetic[moon]).Sum();
    }

    private Answer CalculatePart1Answer()
    {
        return SimulateGravity(1000);
    }

    private Answer CalculatePart2Answer()
    {
        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
