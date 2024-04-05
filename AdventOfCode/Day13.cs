using GridSpot = (int Row, int Column);

namespace AdventOfCode;

public enum Tile
{
    Empty = 0,
    Wall = 1,
    Block = 2,
    Paddle = 3,
    Ball = 4,
}


public sealed class Day13 : BaseTestableDay
{
    private readonly List<long> _input;

    public Day13() : this(RunMode.Real)
    {
    }

    public Day13(RunMode runMode)
    {
        RunMode = runMode;

        _input = File
            .ReadAllLines(InputFilePath)
            .First()
            .Split(',')
            .Select(long.Parse)
            .ToList();
    }

    private Answer CalculatePart1Answer()
    {
        var computer = new Computer(_input);
        var outputs = computer.RunProgramToTermination();

        return outputs.Chunk(3).Count(c => (Tile)c[2] == Tile.Block);
    }

    private char TileToChart(Tile tile)
    {
        return tile switch
        {
            Tile.Empty => ' ',
            Tile.Wall => '#',
            Tile.Block => 'B',
            Tile.Paddle => '_',
            Tile.Ball => '*',
        };
    }

    private (Dictionary<GridSpot, Tile>, long) ConvertOutputToGameBoard(List<long> outputs)
    {
        var board = outputs
            .Chunk(3)
            .Where(c => c[0] != -1)
            .GroupBy(c => new GridSpot((int)c[1], (int)c[0])) // won't need this soon.
            .ToDictionary(
                g => g.Key,
                g => (Tile)g.Last()[2]
            );

        var score = outputs
            .Chunk(3)
            .FirstOrDefault(c => c[0] == -1)?[2];

        return (board, score.GetValueOrDefault(0));
    }

    private void PrintBoard(Dictionary<GridSpot, Tile> board)
    {
        Console.WriteLine();

        var maxRow = board.Keys.Max(s => s.Row);
        var maxColumn = board.Keys.Max(s => s.Column);

        for (int row = 0; row <= maxRow; row++)
        {
            var toPrint = Enumerable.Range(0, maxColumn + 1).Select(column => board[new GridSpot(row, column)]).Select(TileToChart).ToList();
            Console.WriteLine($"{string.Join("", toPrint)}");
        }
    }

    private Answer CalculatePart2Answer()
    {
        var computer = new Computer(_input);
        var outputs = computer.RunProgramToTermination();

        var (board, score) = ConvertOutputToGameBoard(outputs);

        //PrintBoard(board);

        var gameCode = _input.ToList();
        gameCode[0] = 2;

        var inputs = new List<long>();
        var game = new Computer(gameCode, inputs);

        while (true) // Turn loop
        {
            var threeOutputs = new List<long>();

            for (var i = 0; i < 3; i++)
            {
                var (returnMode, result) = game.RunProgram();

                if (returnMode == ReturnMode.Terminate) // This means GAME OVER.
                {
                    //PrintBoard(board);
                    return score;
                }

                if (returnMode == ReturnMode.Input)
                {
                    //PrintBoard(board);

                    var paddle = board.First(kvp => kvp.Value == Tile.Paddle).Key;
                    var ball = board.First(kvp => kvp.Value == Tile.Ball).Key;
                    inputs.Add(ball.Column.CompareTo(paddle.Column)); // -1 for left, 0 for stay, 1 for right.

                    break;
                }

                threeOutputs.Add(result.Value);
            }

            if (threeOutputs.Count != 3) // It's this, or a goto for the game loop. :)
            {
                continue;
            }

            if (threeOutputs[0] == -1) // Score!
            {
                score = threeOutputs.Last();
                //Console.WriteLine($"Score is {score}");

                continue;
            }

            var spot = new GridSpot((int)threeOutputs[1], (int)threeOutputs[0]);
            board[spot] = (Tile)threeOutputs[2];
        }
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
