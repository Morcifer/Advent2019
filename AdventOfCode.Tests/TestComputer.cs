namespace AdventOfCode.Tests;

public class TestComputer
{
    [Theory]
    [InlineData(new [] { 1, 0, 0, 0, 99 }, 2)]
    [InlineData(new[] { 2, 3, 0, 3, 99 }, 2)] // => 2,3,0,6,99
    [InlineData(new[] { 2, 4, 4, 5, 99, 0 }, 2)] // => 2,4,4,5,99,9801
    [InlineData(new[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 }, 30)] // => 30,1,1,4,2,5,6,0,99
    public void ComputerProgramRunAndTestProgramZeroSpot(int[] program, int expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<int>());
        computer.RunProgramToTermination();
        computer.Program[0].Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(new[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 }, 8, 1)]
    [InlineData(new[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 }, 3, 0)]
    [InlineData(new[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 }, 3, 1)]
    [InlineData(new[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 }, 9, 0)]
    [InlineData(new[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 }, 8, 1)]
    [InlineData(new[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 }, 3, 0)]
    [InlineData(new[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 }, 3, 1)]
    [InlineData(new[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 }, 9, 0)]
    [InlineData(new[] { 3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9 }, 0, 0)]
    [InlineData(new[] { 3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9 }, 5, 1)]
    [InlineData(new[] { 3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1 }, 0, 0)]
    [InlineData(new[] { 3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1 }, 5, 1)]
    public void ComputerProgramRunAndTestProgramOutput(int[] program, int input, int expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<int>() { input });
        computer.RunProgramToTermination()[^1].Should().Be(expectedOutput);
    }
}
