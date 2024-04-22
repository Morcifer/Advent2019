using System.Numerics;

namespace AdventOfCode;

public enum ShuffleOption
{
    DealIntoNewStack,
    DealWithIncrement,
    Cut,
}

public sealed class Day22 : BaseTestableDay
{
    private readonly List<(ShuffleOption, int)> _input;

    public Day22() : this(RunMode.Real)
    {
    }

    public Day22(RunMode runMode)
    {
        RunMode = runMode;

        _input = new List<(ShuffleOption, int)>();

        var lines = File.ReadAllLines(InputFilePath);

        foreach (var line in lines)
        {
            if (line == "deal into new stack")
            {
                _input.Add((ShuffleOption.DealIntoNewStack, 0));
                continue;
            }

            if (line.Contains("cut"))
            {
                var cutCards = int.Parse(line.Split(" ").Skip(1).First());
                _input.Add((ShuffleOption.Cut, cutCards));
                continue;
            }

            // "deal with increment N"
            var incrementCards = int.Parse(line.Split(" ").Last());
            _input.Add((ShuffleOption.DealWithIncrement, incrementCards));
        }
    }

    public List<int> ShuffleDeck(List<int> cards)
    {
        var currentCards = cards.ToList();

        foreach (var (shuffleIndex, shuffle) in _input.Enumerate())
        {
            switch (shuffle.Item1)
            {
                case ShuffleOption.DealIntoNewStack:
                    currentCards = currentCards.Reversed().ToList();
                    break;

                case ShuffleOption.Cut:
                    var cut = shuffle.Item2;

                    if (cut < 0)
                    {
                        cut = -cut;
                        currentCards = currentCards.Skip(currentCards.Count - cut).Take(cut).Concat(currentCards.Take(currentCards.Count - cut)).ToList();
                    }
                    else
                    {
                        currentCards = currentCards.Skip(cut).Concat(currentCards.Take(cut)).ToList();
                    }
                    
                    break;

                case ShuffleOption.DealWithIncrement:
                    var increment = shuffle.Item2;
                    var newCards = currentCards.Select(_ => -1).ToList();

                    foreach (var (cardIndex, card) in currentCards.Enumerate())
                    {
                        newCards[cardIndex * increment % currentCards.Count] = card;
                    }

                    currentCards = newCards;
                    break;
            }

            //Console.WriteLine($"Result after {shuffleIndex}: {string.Join(" ", currentCards)}");
        }

        return currentCards;
    }

    private (BigInteger a0, BigInteger a1) GetLinearModulation(BigInteger totalCards)
    {
        // Transformations are a0 + a1 * original_card
        BigInteger a0 = 0;
        BigInteger a1 = 1;

        foreach (var shuffle in _input)
        {
            switch (shuffle.Item1)
            {
                case ShuffleOption.DealIntoNewStack:
                    a1 *= -1;
                    a0 += a1;
                    break;

                case ShuffleOption.Cut:
                    var cut = shuffle.Item2;
                    a0 += (cut * a1);
                    break;

                case ShuffleOption.DealWithIncrement:
                    var increment = shuffle.Item2;
                    a1 *= BigInteger.ModPow(increment, totalCards - 2, totalCards);
                    break;
            }

            a0 = Utilities.PythonMod(a0, totalCards);
            a1 = Utilities.PythonMod(a1, totalCards);
        }

        return (a0, a1);
    }

    private Answer CalculatePart1Answer()
    {
        var cardCount = RunMode == RunMode.Test ? 10 : 10007;
        var cards = Enumerable.Range(0, cardCount).ToList();
        var shuffled = ShuffleDeck(cards);
        var targetCard = RunMode == RunMode.Test ? 7 : 2019;

        return shuffled.FindIndex(x => x == targetCard);
    }

    private Answer CalculatePart2Answer()
    {
        BigInteger cardCount = RunMode == RunMode.Test ? 11 : 119315717514047;  // Needs to be prime for code to work.
        BigInteger targetPosition = RunMode == RunMode.Test ? 7 : 2020;
        BigInteger shuffleCount = RunMode == RunMode.Test ? 2 : 101741582076661;

        if (RunMode == RunMode.Test)
        {
            var cards = Enumerable.Range(0, (int)cardCount).ToList();
            var shuffled = ShuffleDeck(cards);

            for (var i = 1; i < shuffleCount; i++)
            {
                shuffled = ShuffleDeck(shuffled);
            }

            Console.WriteLine($"Result after shuffle: {string.Join(" ", shuffled)}");
            var cardAtSpot = shuffled[(int)targetPosition];
            Console.WriteLine($"Card at spot {targetPosition} started at spot {cardAtSpot}");
        }

        var (a0, a1) = GetLinearModulation(cardCount);

        var multiplicator = BigInteger.ModPow(a1, shuffleCount, cardCount);

        var offset = a0 * (1 - multiplicator) * BigInteger.ModPow(Utilities.PythonMod(1 - a1, cardCount), cardCount - 2, cardCount);
        offset = Utilities.PythonMod(offset, cardCount);

        return (long)Utilities.PythonMod(offset + multiplicator * targetPosition, cardCount);
    }
    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
