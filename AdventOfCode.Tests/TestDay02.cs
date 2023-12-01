namespace AdventOfCode.Tests
{
    public class TestDay02
    {
        [Theory]
        [InlineData(typeof(Day02), RunMode.Test, "3500", "-1")]
        [InlineData(typeof(Day02), RunMode.Real, "10566835", "2347")]
        public async Task Day2_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
