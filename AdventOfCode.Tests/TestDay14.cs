namespace AdventOfCode.Tests;

public class TestDay14
{
    [Theory]
    [InlineData(typeof(Day14), RunMode.Test, "2210736", "-1")]
    [InlineData(typeof(Day14), RunMode.Real, "873899", "-1")]
    public async Task Day14_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
