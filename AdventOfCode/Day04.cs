using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("AdventOfCode.Tests")]

namespace AdventOfCode
{
    public sealed class Day04 : BaseTestableDay
    {
        private readonly (int From, int To) _input;

        public Day04() : this(RunMode.Real)
        {
        }

        public Day04(RunMode runMode)
        {
            RunMode = runMode;

            _input = (248345, 746315);
        }

        internal static bool HasTwoAdjacentDigitsThatAreSame(int password)
        {
            var stringPassword = password.ToString();
            var sameDigits = Enumerable.Range(0, 10).Select(digit => $"{digit}{digit}").ToList();

            return sameDigits.Any(same => stringPassword.Contains(same));
        }

        internal static bool HasExactlyTwoAdjacentDigitsThatAreTheSame(int password)
        {
            var stringPassword = password.ToString();
            var sameTwoDigits = Enumerable.Range(0, 10).Select(digit => $"{digit}{digit}").ToList();
            var sameThreeDigits = Enumerable.Range(0, 10).Select(digit => $"{digit}{digit}{digit}").ToList();

            return sameTwoDigits
                .Zip(sameThreeDigits)
                .Any(same => stringPassword.Contains(same.First) && !stringPassword.Contains(same.Second));
        }

        internal static bool IncreasingDigits(int password)
        {
            var stringPassword = password.ToString();
            for (var i = 0; i <= 4; i++)
            {
                if (stringPassword[i] > stringPassword[i + 1]) // This should work for the chars as well...
                {
                    return false;
                }
            }

            return true;
        }

        private Answer HackPassword()
        {
            return Enumerable
                .Range(_input.From, _input.To - _input.From + 1)
                .Where(HasTwoAdjacentDigitsThatAreSame)
                .Where(IncreasingDigits)
                .Count();
        }

        private Answer HackStrongerPassword()
        {
            return Enumerable
                .Range(_input.From, _input.To - _input.From + 1)
                .Where(HasExactlyTwoAdjacentDigitsThatAreTheSame)
                .Where(IncreasingDigits)
                .Count();
        }

        public override ValueTask<string> Solve_1() => new(HackPassword());

        public override ValueTask<string> Solve_2() => new(HackStrongerPassword());
    }
}