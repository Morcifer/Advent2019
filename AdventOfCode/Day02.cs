using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("AdventOfCode.Tests")]
namespace AdventOfCode
{
    public enum OpCode
    {
        Add = 1,
        Multiply = 2,
        Terminate = 99,
    }

    public sealed class Day02 : BaseTestableDay
    {
        private readonly List<int> _input;

        public Day02() : this(RunMode.Real)
        {
        }

        public Day02(RunMode runMode)
        {
            RunMode = runMode;

            _input = File
                .ReadAllLines(InputFilePath)
                .First()
                .Split(',')
                .Select(int.Parse)
                .ToList();
        }

        private int RunProgram(List<int> program)
        {
            var instructionPointer = 0;

            while (instructionPointer < program.Count)
            {
                int parameter1, parameter2, parameter3;

                switch ((OpCode)program[instructionPointer])
                {
                    case OpCode.Add:
                        parameter1 = program[instructionPointer + 1];
                        parameter2 = program[instructionPointer + 2];
                        parameter3 = program[instructionPointer + 3];

                        program[parameter3] = program[parameter1] + program[parameter2];
                        instructionPointer += 4;

                        break;
                    case OpCode.Multiply:
                        parameter1 = program[instructionPointer + 1];
                        parameter2 = program[instructionPointer + 2];
                        parameter3 = program[instructionPointer + 3];

                        program[parameter3] = program[parameter1] * program[parameter2]; 
                        instructionPointer += 4;

                        break;
                    case OpCode.Terminate:
                        instructionPointer += 1;
                        return program[0];
                    default:
                        throw new ArgumentException("Invalid value for OpCode", nameof(OpCode));
                };
            }

            return -1;
        }

        private int Program1202()
        {
            var program = _input.ToList();
            
            if (RunMode == RunMode.Real)
            {
                program[1] = 12;
                program[2] = 2;
            }

            return RunProgram(program);
        }

        private int FindNounAndVerb()
        {
            for (int noun = 0; noun < _input.Count; noun++)
            {
                for (int verb = 0; verb < _input.Count; verb++)
                {
                    var program = _input.ToList();

                    program[1] = noun;
                    program[2] = verb;

                    try
                    {
                        var result = RunProgram(program);
                        if (result == 19690720)
                        {
                            return 100 * noun + verb;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return -1;
        }


        public override ValueTask<string> Solve_1() => new($"{Program1202()}");

        public override ValueTask<string> Solve_2() => new($"{FindNounAndVerb()}");
    }
}