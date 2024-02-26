namespace AdventOfCode;

public enum OpCode
{
    Add = 1,
    Multiply = 2,
    Store = 3,
    Output = 4,
    JumpIfTrue = 5,
    JumpIfFalse = 6,
    LessThan = 7,
    Equals = 8,
    Terminate = 99,
}

public enum ParameterMode
{
    Position = 0, // Value stored in position in memory
    Immediate = 1, // Value stored as a value
}
    
public class Computer
{
    public List<int> Program { get; private set; }
    private List<int> _inputs;

    private int _instructionPointer;
    private int _inputPointer;

    public Computer(List<int> program, List<int> inputs)
    {
        this.Program = program.ToList();
        this._inputs = inputs;

        this._instructionPointer = 0;
        this._inputPointer = 0;
    }

    private int GetParameter(int parameterId, int code)
    {
        var parameterMode = (ParameterMode)((code / (int)Math.Pow(10, parameterId + 1)) % 10);

        return parameterMode switch
        {
            ParameterMode.Position => Program[Program[_instructionPointer + parameterId]],
            ParameterMode.Immediate => Program[_instructionPointer + parameterId],
            _ => -1,
        };
    }

    public List<int> RunProgramToTermination()
    {
        var outputs = new List<int>();
        while (true)
        {
            var output = this.RunProgram();

            if (!output.HasValue)
            {
                return outputs;
            }

            outputs.Add(output.Value);
        }
    }

    public int? RunProgram()
    {
        while (_instructionPointer < Program.Count)
        {
            int parameter1, parameter2, parameter3;

            var code = Program[_instructionPointer];
            var opCode = (OpCode)(code % 100);

            switch (opCode)
            {
                case OpCode.Add:
                    parameter1 = GetParameter(1, code);
                    parameter2 = GetParameter(2, code);
                    parameter3 = Program[_instructionPointer + 3];

                    Program[parameter3] = parameter1 + parameter2;
                    _instructionPointer += 4;

                    break;
                case OpCode.Multiply:
                    parameter1 = GetParameter(1, code);
                    parameter2 = GetParameter(2, code);
                    parameter3 = Program[_instructionPointer + 3];

                    Program[parameter3] = parameter1 * parameter2;
                    _instructionPointer += 4;

                    break;
                case OpCode.Store:
                    parameter1 = Program[_instructionPointer + 1];

                    Program[parameter1] = _inputs[_inputPointer++];

                    _instructionPointer += 2;

                    break;
                case OpCode.Output:
                    parameter1 = GetParameter(1, code);

                    _instructionPointer += 2;

                    return parameter1;

                case OpCode.Terminate:
                    // ReSharper disable once RedundantAssignment
                    _instructionPointer += 1;
                    return null;

                case OpCode.JumpIfTrue:
                    parameter1 = GetParameter(1, code);
                    parameter2 = GetParameter(2, code);

                    _instructionPointer = parameter1 != 0  ? parameter2 : _instructionPointer + 3;
                    break;
                case OpCode.JumpIfFalse:
                    parameter1 = GetParameter(1, code);
                    parameter2 = GetParameter(2, code);

                    _instructionPointer = parameter1 == 0 ? parameter2 : _instructionPointer + 3;
                    break;
                case OpCode.LessThan:
                    parameter1 = GetParameter(1, code);
                    parameter2 = GetParameter(2, code);
                    parameter3 = Program[_instructionPointer + 3];

                    Program[parameter3] = parameter1 < parameter2 ? 1 : 0;
                    _instructionPointer += 4;
                    break;
                case OpCode.Equals:
                    parameter1 = GetParameter(1, code);
                    parameter2 = GetParameter(2, code);
                    parameter3 = Program[_instructionPointer + 3];

                    Program[parameter3] = parameter1 == parameter2 ? 1 : 0;
                    _instructionPointer += 4;
                    break;
                default:
                    throw new ArgumentException("Invalid value for OpCode", nameof(OpCode));
            }
        }

        throw new ArgumentOutOfRangeException("I should not be getting here!");
    }
}
