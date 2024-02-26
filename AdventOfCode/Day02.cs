namespace AdventOfCode;

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

    private Answer Program1202()
    {
        var program = _input.ToList();
            
        if (RunMode == RunMode.Real)
        {
            program[1] = 12;
            program[2] = 2;
        }
        Computer.RunProgram(program, new List<int>());
        return program[0];
    }

    private Answer FindNounAndVerb()
    {
        for (var noun = 0; noun < _input.Count; noun++)
        {
            for (var verb = 0; verb < _input.Count; verb++)
            {
                var program = _input.ToList();

                program[1] = noun;
                program[2] = verb;

                try
                {
                    Computer.RunProgram(program, new List<int>());

                    if (program[0] == 19690720)
                    {
                        return (100 * noun) + verb;
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
            }
        }

        return -1;
    }


    public override ValueTask<string> Solve_1() => new(Program1202());

    public override ValueTask<string> Solve_2() => new(FindNounAndVerb());
}
