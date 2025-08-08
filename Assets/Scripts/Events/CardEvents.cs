namespace MemoryMatchGame.Events
{
    /// <summary>
    /// Event published when a card is revealed by player interaction.
    /// Contains card identification and visual information.
    /// </summary>
    public readonly struct CardRevealedEvent
    {
        public int CardId { get; }
        public string SpriteId { get; }

        public CardRevealedEvent(int cardId, string spriteId)
        {
            CardId = cardId;
            SpriteId = spriteId;
        }
    }
}