namespace MemoryMatchGame.Events
{
    /// <summary>
    /// Event published when cards are successfully matched.
    /// Contains information about the matched cards and resulting score.
    /// </summary>
    public readonly struct CardsMatchedEvent
    {
        public int[] CardIds { get; }
        public int Score { get; }

        public CardsMatchedEvent(int[] cardIds, int score)
        {
            CardIds = cardIds;
            Score = score;
        }
    }

    /// <summary>
    /// Event published when cards fail to match.
    /// Contains information about the mismatched cards and current error count.
    /// </summary>
    public readonly struct CardsMismatchEvent
    {
        public int[] CardIds { get; }
        public int ErrorCount { get; }

        public CardsMismatchEvent(int[] cardIds, int errorCount)
        {
            CardIds = cardIds;
            ErrorCount = errorCount;
        }
    }
}