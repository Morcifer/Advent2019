namespace AdventOfCode.Tests;

public class TestDay12
{
    [Theory]
    [InlineData(typeof(Day12), RunMode.Test, "183", "2772")]
    [InlineData(typeof(Day12), RunMode.Test2, "14645", "4686774924")]
    [InlineData(typeof(Day12), RunMode.Real, "12070", "500903629351944")]
    public async Task Day12_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
