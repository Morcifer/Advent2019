using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AdventOfCode.Tests")]
namespace AdventOfCode
{
    public sealed class Day01 : BaseTestableDay
    {
        private readonly List<int> _input;

        public Day01() : this(RunMode.Real)
        {
        }

        public Day01(RunMode runMode)
        {
            RunMode = runMode;

            _input = File
                .ReadAllLines(InputFilePath)
                .Select(int.Parse)
                .ToList();
        }

        internal static int CalculateSingleFuel(int mass)
        {
            return (int)Math.Floor(mass / 3.0) - 2;
        }

        internal static int CalculateSingleCompoundFuel(int mass)
        {
            int extraFuel = Math.Max(0, CalculateSingleFuel(mass));
            int totalFuel = extraFuel;

            while (extraFuel > 0)
            {
                extraFuel = Math.Max(0, CalculateSingleFuel(extraFuel));
                totalFuel += extraFuel;
            }

            return totalFuel;
        }

        private int CalculateFuel()
        {
            return _input.Select(CalculateSingleFuel).Sum();
        }

        private int CalculateCompoundFuel()
        {
            return _input.Select(CalculateSingleCompoundFuel).Sum();
        }

        public override ValueTask<string> Solve_1() => new($"{CalculateFuel()}");

        public override ValueTask<string> Solve_2() => new($"{CalculateCompoundFuel()}");
    }
}
