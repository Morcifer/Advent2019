using AdventOfCode;

var inputFilePath = "..\\..\\..\\..\\AdventOfCode\\Inputs\\25.txt";

var program =  File
    .ReadAllLines(inputFilePath)
    .First()
    .Split(',')
    .Select(long.Parse)
    .ToList();

var computer = new Computer(program);

var commands = new List<string>();
var commandIndex = 0;

Console.WriteLine("====================");
Console.WriteLine("LET THE GAMES BEGIN!");
Console.WriteLine("====================");

while (true)
{
    var (returnMode, result) = computer.RunProgramToAsciiNewLine();

    switch (returnMode)
    {
        case ReturnMode.Terminate:
            Console.WriteLine(result);
            return 0;
        case ReturnMode.Output:
            Console.WriteLine(result);
            break;
        case ReturnMode.Input:
            var newCommand = Console.ReadLine();

            if (newCommand is null)
            {
                Console.WriteLine("Invalid command, game over.");
                return 0;
            }

            if (newCommand == "restart")
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("===============");
                Console.WriteLine("RESTARTING GAME");
                Console.WriteLine("===============");

                computer = new Computer(program);

                commands = new List<string>();
                commandIndex = 0;
                break;
            }

            commands.Add(newCommand);

            var command = commands[commandIndex++];
            computer.AddAsciiCommand(command);

            break;
    }
}

