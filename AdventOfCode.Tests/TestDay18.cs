namespace AdventOfCode.Tests;

public class TestDay18
{
    [Theory]
    [InlineData(typeof(Day18), RunMode.Test, "114", "72")]
    [InlineData(typeof(Day18), RunMode.Real, "4246", "-1")]
    public async Task Day18_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
