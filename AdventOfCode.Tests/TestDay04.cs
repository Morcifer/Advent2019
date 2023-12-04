namespace AdventOfCode.Tests
{
    public class TestDay04
    {
        [Theory]
        [InlineData(111111, true)]
        [InlineData(123789, false)]
        public void Day4_HasTwoAdjacentDigitsThatAreSame(int password, bool expected)
        {
            Day04.HasTwoAdjacentDigitsThatAreSame(password).Should().Be(expected);
        }

        [Theory]
        [InlineData(112233, true)]
        [InlineData(123444, false)]
        public void Day4_HasExactlyTwoAdjacentDigitsThatAreTheSame(int password, bool expected)
        {
            Day04.HasExactlyTwoAdjacentDigitsThatAreTheSame(password).Should().Be(expected);
        }

        [Theory]
        [InlineData(111111, true)]
        [InlineData(223450, false)]
        public void Day4_IncreasingDigits(int password, bool expected)
        {
            Day04.IncreasingDigits(password).Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(Day04), RunMode.Real, "1019", "660")]
        public async Task Day4_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
        {
            if (Activator.CreateInstance(type, runMode) is BaseTestableDay instance)
            {
                (await instance.Solve_1()).Should().Be(expectedPart1);
                (await instance.Solve_2()).Should().Be(expectedPart2);
            }
            else
            {
                Assert.Fail($"{type} is not a BaseProblem");
            }
        }
    }
}
