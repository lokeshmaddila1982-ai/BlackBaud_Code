namespace Blackbaud.Interview.Cards;

/// <summary>
/// Strategy for shuffling a list of cards
/// </summary>
public interface IShuffleStrategy
{
    /// <summary>
    /// Shuffle the provided list of cards in-place.
    /// </summary>
    /// <param name="cards">List of cards to shuffle</param>
    void Shuffle(List<Card> cards);
}

/// <summary>
/// Fisher-Yates shuffle strategy. Performs the shuffle the specified number of times.
/// </summary>
public class FisherYatesShuffleStrategy : IShuffleStrategy
{
    private readonly int _times;
    private readonly Random _rng;

    public FisherYatesShuffleStrategy(int times = 1, Random? rng = null)
    {
        if (times < 1) throw new ArgumentOutOfRangeException(nameof(times));
        _times = times;
        _rng = rng ?? Random.Shared;
    }

    public void Shuffle(List<Card> cards)
    {
        if (cards == null) throw new ArgumentNullException(nameof(cards));
        for (int t = 0; t < _times; t++)
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                var tmp = cards[i];
                cards[i] = cards[j];
                cards[j] = tmp;
            }
        }
    }
}

/// <summary>
/// A deck of cards
/// </summary>
public class Deck
{
    private readonly Stack<Card> _stackOfCards;

    /// <summary>
    /// Private constructor for a new deck of <paramref name="cards"/>.
    /// Use Deck.NewDeck() static factory method.
    /// </summary>
    /// <param name="cards"></param>
    private Deck(IEnumerable<Card> cards)
    {
        _stackOfCards = new Stack<Card>(cards);
    }

    /// <summary>
    /// Creates and returns a new deck of cards.
    /// </summary>
    /// <returns></returns>
    public static Deck NewDeck()
    {
        return new Deck(
            Enum.GetValues<Suit>().SelectMany(suit =>
                Enum.GetValues<Rank>().Select(rank =>
                    new Card(rank, suit))
        ));
    }

    /// <summary>
    /// The number of remaining cards in the deck
    /// </summary>
    public int RemainingCards => _stackOfCards.Count;

    /// <summary>
    /// Returns true if there are no remaining cards in the deck
    /// </summary>
    public bool Empty => RemainingCards == 0;

    /// <summary>
    /// Removes the next card from the deck.
    /// </summary>
    /// <returns>The next card from the deck.
    /// Returns null if no cards remain.</returns>
    public Card NextCard()
    {
        if (!Empty)
        {
            var nextCard = _stackOfCards.Pop();
            return nextCard;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Shuffles the deck the specified number of times using Fisher-Yates shuffle.
    /// This overload is kept for convenience and delegates to the strategy-based shuffle.
    /// </summary>
    /// <param name="times">Number of shuffle passes to perform; must be >= 1.</param>
    public void Shuffle(int times = 1)
    {
        Shuffle(new FisherYatesShuffleStrategy(times));
    }

    /// <summary>
    /// Shuffles the deck using the provided shuffle strategy.
    /// </summary>
    /// <param name="strategy">Strategy to perform the shuffle</param>
    public void Shuffle(IShuffleStrategy strategy)
    {
        if (strategy == null) throw new ArgumentNullException(nameof(strategy));

        // Copy current cards into a list (enumeration of Stack is LIFO: top first)
        var list = _stackOfCards.ToList();

        strategy.Shuffle(list);

        // Rebuild the stack so that the first element of the list becomes the next popped card
        _stackOfCards.Clear();
        for (int i = list.Count - 1; i >= 0; i--)
        {
            _stackOfCards.Push(list[i]);
        }
    }

    /// <summary>
    /// Randomizes the order of the cards in the deck using the Fisher-Yates algorithm.
    /// </summary>
    /// <param name="times">The number of times to perform the shuffle pass. Must be &gt;= 1. Default is 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="times"/> is less than 1.</exception>
    /// <remarks>
    /// The method:
    /// - Validates the <paramref name="times"/> argument.
    /// - Copies the internal stack to a list to allow efficient indexed swaps.
    /// - Performs the Fisher-Yates shuffle the specified number of times. Repeating the shuffle
    ///   increases randomness but has linear cost per pass (O(n)).
    /// - Rebuilds the internal stack so that the first element of the shuffled list becomes the next
    ///   card returned by <see cref="NextCard"/>.
    /// Uses <see cref="Random.Shared"/> for a thread-safe random number generator.
    /// </remarks>
    /*
    public void Shuffle(int times = 1)
    {
        if (times < 1)
            throw new ArgumentOutOfRangeException(nameof(times), "times must be >= 1");

        var rng = Random.Shared;

        var lstCards = _stackOfCards.ToList();

        // Fisher-Yates shuffle performed 'times' times.
        for (int t = 0; t < times; t++)
        {
            for (int i = lstCards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var temp = lstCards[i];
                lstCards[i] = lstCards[j];
                lstCards[j] = temp;
            }
        }

        // Rebuild the stack so that list[0] becomes the next popped card.
        _stackOfCards.Clear();

        for (int i = lstCards.Count - 1; i >= 0; i--)
        {
            _stackOfCards.Push(lstCards[i]);
        }
    }
    */
}
