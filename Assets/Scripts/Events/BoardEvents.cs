using MemoryMatchGame.Core.Configuration;

namespace MemoryMatchGame.Events
{
    /// <summary>
    /// Event published when the game board is successfully initialized.
    /// Contains configuration information for subscribers to react accordingly.
    /// </summary>
    public readonly struct BoardInitializedEvent
    {
        public BoardConfiguration Config { get; }

        public BoardInitializedEvent(BoardConfiguration config)
        {
            Config = config;
        }
    }
}