namespace AdventOfCode
{
    public enum OpCode
    {
        Add = 1,
        Multiply = 2,
        Terminate = 99,
    }

    public static class Computer
    {
        public static int RunProgram(List<int> program)
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
                        // ReSharper disable once RedundantAssignment
                        instructionPointer += 1;
                        return program[0];
                    default:
                        throw new ArgumentException("Invalid value for OpCode", nameof(OpCode));
                }
            }

            return -1;
        }
    }
}
