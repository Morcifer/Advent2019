namespace AdventOfCode.Tests;

public class TestComputer
{
    [Theory]
    [InlineData(new long[] { 1, 0, 0, 0, 99 }, 2)]
    [InlineData(new long[] { 2, 3, 0, 3, 99 }, 2)] // => 2,3,0,6,99
    [InlineData(new long[] { 2, 4, 4, 5, 99, 0 }, 2)] // => 2,4,4,5,99,9801
    [InlineData(new long[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 }, 30)] // => 30,1,1,4,2,5,6,0,99
    public void ComputerProgramRunAndTestProgramZeroSpot(long[] program, long expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<long>());
        computer.RunProgramToTermination();
        computer.Program[0].Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(new long[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 }, 8, 1)]
    [InlineData(new long[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 }, 3, 0)]
    [InlineData(new long[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 }, 3, 1)]
    [InlineData(new long[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 }, 9, 0)]
    [InlineData(new long[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 }, 8, 1)]
    [InlineData(new long[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 }, 3, 0)]
    [InlineData(new long[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 }, 3, 1)]
    [InlineData(new long[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 }, 9, 0)]
    [InlineData(new long[] { 3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9 }, 0, 0)]
    [InlineData(new long[] { 3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9 }, 5, 1)]
    [InlineData(new long[] { 3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1 }, 0, 0)]
    [InlineData(new long[] { 3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1 }, 5, 1)]
    public void ComputerProgramRunAndTestProgramOutput(long[] program, long input, long expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<long>() { input });
        computer.RunProgramToTermination()[^1].Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(new long[] { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 }, new long[] { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 })]
    [InlineData(new long[] { 1102, 34915192, 34915192, 7, 4, 7, 99, 0 }, new long[] { 1219070632396864 })] // 12-digit number
    [InlineData(new long[] { 104, 1125899906842624, 99 }, new long[] { 1125899906842624 })]
    public void ComputerProgramRunWithSomeLargeNumbersAndExtraStuff(long[] program, long[] expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<long>());
        computer.RunProgramToTermination().Should().Equal(expectedOutput);
    }

    [Theory]
    [InlineData(new long[] { 109, -1, 4, 1, 99 }, new long[] { -1 })]
    [InlineData(new long[] { 109, -1, 104, 1, 99 }, new long[] { 1 })]
    [InlineData(new long[] { 109, -1, 204, 1, 99 }, new long[] { 109 })]
    [InlineData(new long[] { 109, 1, 9, 2, 204, -6, 99 }, new long[] { 204 })]
    [InlineData(new long[] { 109, 1, 109, 9, 204, -6, 99 }, new long[] { 204 })]
    [InlineData(new long[] { 109, 1, 209, -1, 204, -106, 99 }, new long[] { 204 })]
    public void ComputerProgramRunHailMary(long[] program, long[] expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<long>());
        computer.RunProgramToTermination().Should().Equal(expectedOutput);
    }

    [Theory]
    [InlineData(new long[] { 109, 1, 3, 3, 204, 2, 99 }, 1, new long[] { 1 })]
    [InlineData(new long[] { 109, 1, 203, 2, 204, 2, 99 }, 1, new long[] { 1 })]
    public void ComputerProgramRunHailMaryWithInput(long[] program, long input, long[] expectedOutput)
    {
        var computer = new Computer(program.ToList(), new List<long>() { input });
        computer.RunProgramToTermination().Should().Equal(expectedOutput);
    }
}
