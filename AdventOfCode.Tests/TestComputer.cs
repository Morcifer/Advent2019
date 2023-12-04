namespace AdventOfCode.Tests
{
    public class TestComputer
    {
        [Theory]
        [InlineData(new [] { 1, 0, 0, 0, 99 }, 2)]
        [InlineData(new[] { 2, 3, 0, 3, 99 }, 2)] // => 2,3,0,6,99
        [InlineData(new[] { 2, 4, 4, 5, 99, 0 }, 2)] // => 2,4,4,5,99,9801
        [InlineData(new[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 }, 30)] // => 30,1,1,4,2,5,6,0,99
        public void Day1_Part1_SingleRowCalculation(int[] program, int expectedOutput)
        {
            var realProgram = program.ToList();
            Computer.RunProgram(realProgram, -1);
            realProgram[0].Should().Be(expectedOutput);
        }
    }
}
