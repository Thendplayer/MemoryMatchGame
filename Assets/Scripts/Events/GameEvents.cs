namespace MemoryMatchGame.Events
{
    /// <summary>
    /// Event published when the game is won.
    /// Contains the final score achieved by the player.
    /// </summary>
    public readonly struct GameWonEvent
    {
        public int FinalScore { get; }

        public GameWonEvent(int finalScore)
        {
            FinalScore = finalScore;
        }
    }
}