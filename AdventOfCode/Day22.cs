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

    public Day22() : this(RunMode.Test)
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

    public long ShuffleSingleCard(long totalCards, long targetIndex)
    {
        var currentIndex = targetIndex;

        foreach (var (shuffleIndex, shuffle) in _input.Enumerate())
        {
            switch (shuffle.Item1)
            {
                case ShuffleOption.DealIntoNewStack:
                    currentIndex = totalCards - 1 - currentIndex;
                    break;

                case ShuffleOption.Cut:
                    var cut = shuffle.Item2;
                    currentIndex = (currentIndex - cut + totalCards) % totalCards;
                    break;

                case ShuffleOption.DealWithIncrement:
                    var increment = shuffle.Item2;
                    currentIndex = currentIndex * increment % totalCards;
                    break;
            }

            //Console.WriteLine($"Result after {shuffleIndex}: {string.Join(" ", currentIndex)}");
        }

        return currentIndex;
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
        var cardCount = RunMode == RunMode.Test ? 10 : 119315717514047;
        var targetCard = RunMode == RunMode.Test ? 7 : (long)2020;
        var shuffleCount = RunMode == RunMode.Test ? 1 : 101741582076661;

        var targetCardShuffles = new Dictionary<long, long>();

        for (var shuffle = 0; shuffle < shuffleCount; shuffle++)
        {
            targetCardShuffles[targetCard] = shuffle;

            targetCard = ShuffleSingleCard(cardCount, targetCard);
            //Console.WriteLine($"Result after shuffle {shuffle}: {targetCard}");
            if (shuffle % 100000 == 0)
            {
                Console.WriteLine($"Result after shuffle {shuffle}: {targetCard}");
            }
        }

        return targetCard;
    }

    public override ValueTask<string> Solve_1() => CalculatePart1Answer();

    public override ValueTask<string> Solve_2() => CalculatePart2Answer();
}
