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
        var computer = new Computer(_input, new List<long>());
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

    private (Dictionary<GridSpot, Tile>, long?) ConvertOutputToGameBoard(List<long> outputs)
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

        return (board, score);
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
        var computer = new Computer(_input, new List<long>());
        var outputs = computer.RunProgramToTermination();

        var (board, _) = ConvertOutputToGameBoard(outputs);

        PrintBoard(board);

        var gameCode = _input.ToList();
        gameCode[0] = 2;

        var inputs = new List<long>() { -1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        var game = new Computer(gameCode, inputs);

        while (true) // Turn loop
        {
            var currentOutputs = new List<long>();

            while (true) // Output loop
            {
                var (returnMode, result) = game.RunProgram();

                if (returnMode == ReturnMode.Terminate) // This means GAME OVER. Which suggests that we're getting an update, not whole screens.
                {
                    break;
                }
                else
                {
                    currentOutputs.Add(result.Value);
                }
            }

            var (currentBoard, currentScore) = ConvertOutputToGameBoard(currentOutputs);
            PrintBoard(currentBoard);
        }

        return -1;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
