using System;

namespace MemoryMatchGame.Core.Configuration
{
    /// <summary>
    /// Value object representing the board's structural configuration.
    /// Immutable configuration that defines the game board dimensions and match requirements.
    /// </summary>
    [Serializable]
    public readonly struct BoardConfiguration
    {
        public int Width { get; }
        public int Height { get; }
        public int MatchRequirement { get; }

        public BoardConfiguration(int width, int height, int matchRequirement = 2)
        {
            Width = width;
            Height = height;
            MatchRequirement = matchRequirement;
        }

        /// <summary>
        /// Calculates the total number of cards based on board dimensions.
        /// </summary>
        public int TotalCards => Width * Height;

        /// <summary>
        /// Validates if the board configuration allows for valid pairs.
        /// </summary>
        public bool IsValidConfiguration()
        {
            return TotalCards > 0 && 
                   TotalCards % 2 == 0 && 
                   MatchRequirement > 1 && 
                   MatchRequirement <= 4;
        }
    }
}