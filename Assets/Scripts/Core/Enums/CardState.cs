namespace MemoryMatchGame.Core.Enums
{
    /// <summary>
    /// Represents the current state of a card in the game.
    /// States follow a specific flow: Hidden -> Revealed -> (Matched or back to Hidden)
    /// </summary>
    public enum CardState
    {
        /// <summary>Card is face-down and not visible to the player</summary>
        Hidden,
        
        /// <summary>Card is face-up and visible to the player</summary>
        Revealed,
        
        /// <summary>Card has been successfully matched and is permanently visible</summary>
        Matched
    }
}