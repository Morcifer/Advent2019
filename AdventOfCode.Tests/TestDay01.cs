namespace AdventOfCode.Tests
{
    public class TestDay01
    {
        [Theory]
        [InlineData(12, 2)]
        [InlineData(14, 2)]
        [InlineData(1969, 654)]
        [InlineData(100756, 33583)]
        public void Day1_Part1_SingleRowCalculation(int mass, int expectedFuel)
        {
            Day01.CalculateSingleFuel(mass).Should().Be(expectedFuel);
        }

        [Theory]
        [InlineData(12, 2)]
        [InlineData(14, 2)]
        [InlineData(1969, 966)]
        [InlineData(100756, 50346)]
        public void Day1_Part2_SingleRowCalculation(int mass, int expectedFuel)
        {
            Day01.CalculateSingleCompoundFuel(mass).Should().Be(expectedFuel);
        }

        [Theory]
        [InlineData(typeof(Day01), RunMode.Test, "34241", "51316")]
        [InlineData(typeof(Day01), RunMode.Real, "3394032", "5088176")]
        public async Task Day1_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
