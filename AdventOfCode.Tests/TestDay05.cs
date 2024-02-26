namespace AdventOfCode.Tests;

public class TestDay05
{
    [Theory]
    [InlineData(typeof(Day05), RunMode.Test, "1", "5")]
    [InlineData(typeof(Day05), RunMode.Real, "13285749", "5000972")]
    public async Task Day5_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
