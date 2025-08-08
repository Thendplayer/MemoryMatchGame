using System;
using System.Collections.Generic;

namespace MemoryMatchGame.Core.Configuration
{
    /// <summary>
    /// Value object representing a visual theme configuration.
    /// Contains theme identification and available sprites for card generation.
    /// </summary>
    [Serializable]
    public readonly struct ThemeConfiguration
    {
        public string ThemeId { get; }
        public IReadOnlyList<string> CardSprites { get; }

        public ThemeConfiguration(string themeId, IReadOnlyList<string> cardSprites)
        {
            ThemeId = themeId ?? throw new ArgumentNullException(nameof(themeId));
            CardSprites = cardSprites ?? throw new ArgumentNullException(nameof(cardSprites));
        }

        /// <summary>
        /// Validates if the theme has sufficient sprites for the given board configuration.
        /// </summary>
        public bool HasSufficientSprites(BoardConfiguration boardConfig)
        {
            var requiredUniqueSprites = boardConfig.TotalCards / 2;
            return CardSprites.Count >= requiredUniqueSprites;
        }

        /// <summary>
        /// Validates if the theme configuration is complete and valid.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ThemeId) && 
                   CardSprites != null && 
                   CardSprites.Count > 0;
        }
    }
}