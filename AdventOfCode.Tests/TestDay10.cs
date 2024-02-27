namespace AdventOfCode.Tests;

public class TestDay10
{
    [Theory]
    [InlineData(typeof(Day10), RunMode.Test, "210", "802")]
    [InlineData(typeof(Day10), RunMode.Real, "326", "1623")]
    public async Task Day10_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
