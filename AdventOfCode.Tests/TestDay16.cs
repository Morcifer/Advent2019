namespace AdventOfCode.Tests;

public class TestDay16
{
    [Theory]
    [InlineData("12345678", 1, "48226158")]
    [InlineData("48226158", 1, "34040438")]
    [InlineData("34040438", 1, "03415518")]
    [InlineData("03415518", 1, "01029498")]
    [InlineData("80871224585914546619083218645595", 100, "24176176")]
    [InlineData("19617804207202209144916044189917", 100, "73745418")]
    [InlineData("69317163492948606335995924319873", 100, "52432133")]
    public void Day16_CalculatePhases(string input, int phases, string expected)
    {
        Day16.CalculatePhases(input, phases, 1, 0)[..8].Should().Be(expected);
    }

    [Theory]
    [InlineData("03036732577212944063491565474664", "84462026")]
    [InlineData("02935109699940807407585447034323", "78725270")]
    [InlineData("03081770884921959731165446850517", "53553731")]
    public void Day16_CalculatePhasesWithSkip(string input, string expected)
    {
        Day16.CalculatePhasesWithSkip(input, 100, 10000)[..8].Should().Be(expected);
    }

    [Theory]
    [InlineData(typeof(Day16), RunMode.Real, "61149209", "-1")]
    public async Task Day16_Regression(Type type, RunMode runMode, string expectedPart1, string expectedPart2)
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
