namespace AdventOfCode
{
    public enum OpCode
    {
        Add = 1,
        Multiply = 2,
        Store = 3,
        Output = 4,
        Terminate = 99,
    }

    public enum ParameterMode
    {
        Position = 0, // Value stored in position in memory
        Immediate = 1, // Value stored as a value
    }
    
    public static class Computer
    {
        private static int GetParameter(List<int> program, int instructionPointer, int parameterId, int code)
        {
            var parameterMode = (ParameterMode)((code / (int)Math.Pow(10, parameterId + 1)) % 10);

            return parameterMode switch
            {
                ParameterMode.Position => program[program[instructionPointer + parameterId]],
                ParameterMode.Immediate => program[instructionPointer + parameterId],
                _ => -1,
            };
        }

        public static List<int> RunProgram(List<int> program, int inputValue)
        {
            var outputs = new List<int>();
            var instructionPointer = 0;

            while (instructionPointer < program.Count)
            {
                int parameter1, parameter2, parameter3;

                var code = program[instructionPointer];
                var opCode = (OpCode)(code % 100);

                switch (opCode)
                {
                    case OpCode.Add:
                        parameter1 = GetParameter(program, instructionPointer, 1, code);
                        parameter2 = GetParameter(program, instructionPointer, 2, code);
                        parameter3 = program[instructionPointer + 3];

                        program[parameter3] = parameter1 + parameter2;
                        instructionPointer += 4;

                        break;
                    case OpCode.Multiply:
                        parameter1 = GetParameter(program, instructionPointer, 1, code);
                        parameter2 = GetParameter(program, instructionPointer, 2, code);
                        parameter3 = program[instructionPointer + 3];

                        program[parameter3] = parameter1 * parameter2;
                        instructionPointer += 4;

                        break;
                    case OpCode.Store:
                        parameter1 = program[instructionPointer + 1];

                        program[parameter1] = inputValue;
                        instructionPointer += 2;

                        break;
                    case OpCode.Output:
                        parameter1 = GetParameter(program, instructionPointer, 1, code);

                        outputs.Add(parameter1);

                        instructionPointer += 2;
                        break;

                    case OpCode.Terminate:
                        // ReSharper disable once RedundantAssignment
                        instructionPointer += 1;
                        return outputs;

                    default:
                        throw new ArgumentException("Invalid value for OpCode", nameof(OpCode));
                }
            }

            return outputs;
        }
    }
}
